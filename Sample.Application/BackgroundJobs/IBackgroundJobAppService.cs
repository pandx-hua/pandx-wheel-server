using pandx.Wheel.Application.Dto;
using pandx.Wheel.Application.Services;
using pandx.Wheel.Models;
using Sample.Application.BackgroundJobs.Dto;

namespace Sample.Application.BackgroundJobs;

public interface IBackgroundJobAppService : IApplicationService
{
    Task CreateOrUpdateBackgroundJobAsync(CreateOrUpdateBackgroundJobRequest request);
    Task DeleteBackgroundJobAsync(EntityDto<Guid> request);
    Task<ListResponse<KeyValuePair<string, string>>> GetExposedBackgroundJobsAsync();
    Task ChangeBackgroundJobStatusAsync(ChangeBackgroundJobStatusRequest request);
    Task<PagedResponse<BackgroundJobsDto>> GetPagedBackgroundJobsAsync(GetBackgroundJobsRequest request);
    Task<ListResponse<JobExecutionsDto>> GetJobExecutionsAsync(GetJobExecutionsRequest request);
    Task<ValidationResponse> ValidateJobNameAsync(ValidationRequest<string, string> request);
    Task<ValidationResponse> ValidateCronExpressionAsync(ValidationRequest<string, string> request);
}