using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Events;
using Quartz;

namespace pandx.Wheel.BackgroundJobs;

public class BackgroundJobListener : IJobListener
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<BackgroundJobListener> _logger;
    private readonly IServiceProvider _serviceProvider;

    public BackgroundJobListener(
        IEventPublisher eventPublisher,
        IServiceProvider serviceProvider,
        ILogger<BackgroundJobListener> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new())
    {
        if (context.JobDetail.JobType.IsDefined(typeof(ExposedJobAttribute), true))
        {
            await _eventPublisher.PublishAsync(new JobToBeExecutedEvent(context));
            using var scope = _serviceProvider.CreateScope();
            var backgroundJobInfoRepository =
                scope.ServiceProvider.GetRequiredService<IRepository<BackgroundJobInfo, Guid>>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var jobExecutionInfoRepository =
                scope.ServiceProvider.GetRequiredService<IRepository<JobExecutionInfo, Guid>>();
            var backgroundJobInfo =
                await backgroundJobInfoRepository.GetAsync(Guid.Parse(context.JobDetail.Key.Name));
            _logger.LogInformation($"定时任务 {backgroundJobInfo.Id}/{backgroundJobInfo.JobName} 即将执行....");
            backgroundJobInfo.PrevRunTime = context.PreviousFireTimeUtc?.ToLocalTime().DateTime;
            var now = DateTime.Now;
            backgroundJobInfo.LastRunTime = now;
            backgroundJobInfo.NextRunTime = context.NextFireTimeUtc?.ToLocalTime().DateTime;
            await jobExecutionInfoRepository.InsertAsync(new JobExecutionInfo
            {
                BackgroundJobId = backgroundJobInfo.Id,
                JobName = backgroundJobInfo.JobName,
                StartTime = now,
                EndTime = now
            });
            await unitOfWork.CommitAsync();
        }
        else
        {
            _logger.LogInformation($"普通任务 {context.JobDetail.Key.Name} 即将执行....");
        }
    }

    public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new())
    {
        if (context.JobDetail.JobType.IsDefined(typeof(ExposedJobAttribute), true))
        {
            await _eventPublisher.PublishAsync(new JobExecutionVetoedEvent(context));
            _logger.LogInformation($"定时任务 {context.JobDetail.Key.Name} 被否决....");
        }
        else
        {
            _logger.LogInformation($"普通任务 {context.JobDetail.Key.Name} 被否决....");
        }
    }

    public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
        CancellationToken cancellationToken = new())
    {
        if (context.JobDetail.JobType.IsDefined(typeof(ExposedJobAttribute), true))
        {
            await _eventPublisher.PublishAsync(new JobWasExecutedEvent(context, jobException));
            using var scope = _serviceProvider.CreateScope();
            var backgroundJobInfoRepository =
                scope.ServiceProvider.GetRequiredService<IRepository<BackgroundJobInfo, Guid>>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var jobExecutionInfoRepository =
                scope.ServiceProvider.GetRequiredService<IRepository<JobExecutionInfo, Guid>>();
            var backgroundJobInfo =
                await backgroundJobInfoRepository.GetAsync(Guid.Parse(context.JobDetail.Key.Name));
            var jobExecutionInfo =
                await (await jobExecutionInfoRepository.GetAllAsync(j => j.BackgroundJobId == backgroundJobInfo.Id))
                    .OrderByDescending(j => j.StartTime).FirstAsync();
            var now = DateTime.Now;
            if (jobException is not null)
            {
                _logger.LogInformation($"定时任务 {backgroundJobInfo.Id}/{backgroundJobInfo.JobName} 执行失败....");
                backgroundJobInfo.FailCount++;
                jobExecutionInfo.EndTime = now;
                jobExecutionInfo.Result = false;
                jobExecutionInfo.Exception = JsonConvert.SerializeObject(jobException);
                jobExecutionInfo.Duration = (now - jobExecutionInfo.StartTime).Milliseconds;
            }
            else
            {
                _logger.LogInformation($"定时任务 {backgroundJobInfo.Id}/{backgroundJobInfo.JobName} 执行完毕....");
                backgroundJobInfo.SuccessCount++;
                jobExecutionInfo.EndTime = now;
                jobExecutionInfo.Result = true;
                jobExecutionInfo.Duration = (now - jobExecutionInfo.StartTime).Milliseconds;
            }

            var expiredEntries = (await jobExecutionInfoRepository.GetAllAsync()).OrderByDescending(j => j.StartTime)
                .Skip(99);
            await expiredEntries.ExecuteDeleteAsync(cancellationToken);
            await unitOfWork.CommitAsync();
        }
        else
        {
            _logger.LogInformation(jobException is not null
                ? $"普通任务 {context.JobDetail.Key.Name} 执行失败...."
                : $"普通任务 {context.JobDetail.Key.Name} 执行完毕....");
        }
    }

    public string Name { get; } = nameof(BackgroundJobListener);
}