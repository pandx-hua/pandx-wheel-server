using System.Linq.Dynamic.Core;
using System.Reflection;
using Cronos;
using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.Extensions;
using pandx.Wheel.Helpers;
using pandx.Wheel.Models;
using Quartz;
using Sample.Application.BackgroundJobs.Dto;
using CronExpression = Cronos.CronExpression;

namespace Sample.Application.BackgroundJobs;

public class BackgroundJobAppService : SampleAppServiceBase, IBackgroundJobAppService
{
    private readonly IBackgroundJobLauncher _backgroundJobLauncher;
    private readonly IBackgroundJobManager _backgroundJobManager;

    public BackgroundJobAppService(IBackgroundJobManager backgroundJobManager,
        IBackgroundJobLauncher backgroundJobLauncher)
    {
        _backgroundJobManager = backgroundJobManager;
        _backgroundJobLauncher = backgroundJobLauncher;
    }

    public async Task<ListResponse<JobExecutionsDto>> GetJobExecutionsAsync(GetJobExecutionsRequest request)
    {
        var jobExecutions = await _backgroundJobManager.GetJobExecutionsAsync(request.BackgroundJobId);
        var dtos = jobExecutions.Select(item =>
        {
            var dto = Mapper.Map<JobExecutionsDto>(item);
            return dto;
        }).ToList();
        return new ListResponse<JobExecutionsDto>(dtos);
    }

    public async Task<PagedResponse<BackgroundJobsDto>> GetPagedBackgroundJobsAsync(GetBackgroundJobsRequest request)
    {
        var query = await GetBackgroundJobsQuery(request);
        var totalCount = await query.CountAsync();
        var backgroundJobs = await query.OrderBy(request.Sorting!).PageBy(request).ToListAsync();
        var dtos = backgroundJobs.Select(item =>
        {
            var dto = Mapper.Map<BackgroundJobsDto>(item);
            return dto;
        }).ToList();
        return new PagedResponse<BackgroundJobsDto>(totalCount, dtos);
    }

    public async Task CreateOrUpdateBackgroundJobAsync(CreateOrUpdateBackgroundJobRequest request)
    {
        if (request.BackgroundJob.Id is not null)
        {
            await UpdateBackgroundJobAsync(request);
        }
        else
        {
            await CreateBackgroundJobAsync(request);
        }
    }

    public async Task DeleteBackgroundJobAsync(EntityDto<Guid> request)
    {
        await _backgroundJobManager.DeleteAsync(request.Id);
        await _backgroundJobLauncher.StopAsync(request.Id.ToString());
    }

    public async Task<ListResponse<KeyValuePair<string, string>>> GetExposedBackgroundJobsAsync()
    {
        // var exposedBackgroundJobs = AssemblyHelper.GetReferencedAssemblies()
        //     .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(ExposedJobAttribute), true)));
        // return Task.FromResult(new ListResponse<KeyValuePair<string, string>>(exposedBackgroundJobs
        //     .ToDictionary(t => t.FullName!,
        //         t => $"{t.GetCustomAttribute<ExposedJobAttribute>()!.Description}-{t.FullName}").ToList()));
        return new ListResponse<KeyValuePair<string, string>>((await _backgroundJobManager.GetExposedBackgroundJobsAsync()).ToList());
    }

    public async Task ChangeBackgroundJobStatusAsync(ChangeBackgroundJobStatusRequest request)
    {
        var backgroundJob = await _backgroundJobManager.GetAsync(request.Id) ?? throw new Exception("没有发现指定的定时任务");
        if (!request.Status)
        {
            backgroundJob.Status = false;
            await _backgroundJobManager.UpdateAsync(backgroundJob);
            await _backgroundJobLauncher.StopAsync(request.Id.ToString());
        }
        else
        {
            backgroundJob.Status = true;
            await _backgroundJobManager.UpdateAsync(backgroundJob);
            await StartBackgroundJobAsync(backgroundJob.Job, new BackgroundJobData
            {
                Id = backgroundJob.Id.ToString(),
                CronExpression = backgroundJob.CronExpression
            });
        }
    }

    public async Task<ValidationResponse> ValidateJobNameAsync(ValidationRequest<string, string> request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            var one = await (await _backgroundJobManager.GetAllAsync()).IgnoreQueryFilters()
                .SingleOrDefaultAsync(r => r.JobName == request.Value);
            if (one != null)
            {
                return new ValidationResponse
                {
                    Status = false,
                    Message = "任务名称 " + request.Value + " 已被占用"
                };
            }

            return new ValidationResponse
            {
                Status = true
            };
        }
        else
        {
            var one = await (await _backgroundJobManager.GetAllAsync()).IgnoreQueryFilters().SingleOrDefaultAsync(r =>
                r.JobName == request.Value && r.Id.ToString() != request.Id);
            if (one != null)
            {
                return new ValidationResponse
                {
                    Status = false,
                    Message = "任务名称 " + request.Value + " 已被占用"
                };
            }

            return new ValidationResponse
            {
                Status = true
            };
        }
    }

    public Task<ValidationResponse> ValidateCronExpressionAsync(ValidationRequest<string, string> request)
    {
        try
        {
            CronExpression.Parse(request.Value, CronFormat.IncludeSeconds);
            return Task.FromResult(new ValidationResponse
            {
                Status = true
            });
        }
        catch (Exception e)
        {
            return Task.FromResult(new ValidationResponse
            {
                Status = false,
                Message = "CRON表达式格式不正确"
            });
        }
    }

    private async Task UpdateBackgroundJobAsync(CreateOrUpdateBackgroundJobRequest request)
    {
        var backgroundJob = await _backgroundJobManager.GetAsync(request.BackgroundJob.Id!.Value) ??
                            throw new Exception("没有发现指定的定时任务");
        if (backgroundJob.Status)
        {
            await _backgroundJobLauncher.StopAsync(request.BackgroundJob.Id!.Value.ToString());
        }

        Mapper.Map(request.BackgroundJob, backgroundJob);
        await _backgroundJobManager.UpdateAsync(backgroundJob);
        if (backgroundJob.Status)
        {
            await StartBackgroundJobAsync(backgroundJob.Job, new BackgroundJobData
            {
                Id = backgroundJob.Id.ToString(),
                CronExpression = backgroundJob.CronExpression
            });
        }
    }

    private async Task CreateBackgroundJobAsync(CreateOrUpdateBackgroundJobRequest request)
    {
        var backgroundJob = Mapper.Map<BackgroundJobInfo>(request.BackgroundJob);
        if (request.IsStartNow)
        {
            backgroundJob.Status = true;
        }

        await _backgroundJobManager.CreateAsync(backgroundJob);

        if (request.IsStartNow)
        {
            await StartBackgroundJobAsync(backgroundJob.Job, new BackgroundJobData
            {
                Id = backgroundJob.Id.ToString(),
                CronExpression = backgroundJob.CronExpression
            });
        }
    }

    private async Task StartBackgroundJobAsync(string job, BackgroundJobData data)
    {
        // var jobType = AssemblyHelper.GetReferencedAssemblies()
        //     .SelectMany(a => a.GetTypes().Where(t => t.FullName == job))
        //     .FirstOrDefault();
        var jobType = Type.GetType(job);
        if (jobType is null)
        {
            throw new Exception($"找不到类型 {job}");
        }

        if (!typeof(IJob).IsAssignableFrom(jobType))
        {
            throw new Exception("类型没有实现 IJob");
        }

        var method = typeof(IBackgroundJobLauncher).GetMethod("StartAsync");
        var genericMethod = method!.MakeGenericMethod(jobType);
        await (Task)genericMethod.Invoke(_backgroundJobLauncher, new object[] { data })!;
    }

    private async Task<IQueryable<BackgroundJobInfo>> GetBackgroundJobsQuery(GetBackgroundJobsRequest request)
    {
        var query = (await _backgroundJobManager.GetAllAsync())
            .Where(x => x.CreationTime >= request.StartTime && x.CreationTime <= request.EndTime)
            .WhereIf(request.Status.Count > 0, x => request.Status.Contains(x.Status))
            .WhereIf(!string.IsNullOrWhiteSpace(request.Filter),
                x => x.JobName!.Contains(request.Filter!) ||
                     (x.Description != null && x.Description.Contains(request.Filter!)));
        return query;
    }
}