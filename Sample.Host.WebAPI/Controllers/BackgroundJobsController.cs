using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Controllers;
using pandx.Wheel.Models;
using Sample.Application.BackgroundJobs;
using Sample.Application.BackgroundJobs.Dto;
using Sample.Domain.Authorization.Permissions;

namespace Sample.Host.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class BackgroundJobsController : WheelControllerBase
{
    private readonly IBackgroundJobAppService _backgroundJobAppService;

    public BackgroundJobsController(IBackgroundJobAppService backgroundJobAppService)
    {
        _backgroundJobAppService = backgroundJobAppService;
    }

    [HttpPost(Name = nameof(CreateOrUpdateBackgroundJob))]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Create)]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Update)]
    public async Task CreateOrUpdateBackgroundJob(CreateOrUpdateBackgroundJobRequest request)
    {
        await _backgroundJobAppService.CreateOrUpdateBackgroundJobAsync(request);
    }

    [HttpPost(Name = nameof(DeleteBackgroundJob))]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Delete)]
    public async Task DeleteBackgroundJob(EntityDto<Guid> request)
    {
        await _backgroundJobAppService.DeleteBackgroundJobAsync(request);
    }

    [HttpPost(Name = nameof(GetExposedBackgroundJobs))]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Search)]
    public Task<ListResponse<KeyValuePair<string, string>>> GetExposedBackgroundJobs()
    {
        return _backgroundJobAppService.GetExposedBackgroundJobsAsync();
    }

    [HttpPost(Name = nameof(ChangeBackgroundJobStatus))]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Change)]
    public async Task ChangeBackgroundJobStatus(ChangeBackgroundJobStatusRequest request)
    {
        await _backgroundJobAppService.ChangeBackgroundJobStatusAsync(request);
    }

    [HttpPost(Name = nameof(GetPagedBackgroundJobs))]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Search)]
    public async Task<PagedResponse<BackgroundJobsDto>> GetPagedBackgroundJobs(GetBackgroundJobsRequest request)
    {
        return await _backgroundJobAppService.GetPagedBackgroundJobsAsync(request);
    }

    [HttpPost(Name = nameof(GetJobExecutions))]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Search)]
    public async Task<ListResponse<JobExecutionsDto>> GetJobExecutions(GetJobExecutionsRequest request)
    {
        return await _backgroundJobAppService.GetJobExecutionsAsync(request);
    }

    [HttpPost(Name = nameof(ValidateJobName))]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Create)]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Update)]
    public async Task<ValidationResponse> ValidateJobName(ValidationRequest<string, string> request)
    {
        return await _backgroundJobAppService.ValidateJobNameAsync(request);
    }

    [HttpPost(Name = nameof(ValidateCronExpression))]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Create)]
    [NeedPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Update)]
    public async Task<ValidationResponse> ValidateCronExpression(ValidationRequest<string, string> request)
    {
        return await _backgroundJobAppService.ValidateCronExpressionAsync(request);
    }
}