namespace Sample.Application.BackgroundJobs.Dto;

public class CreateOrUpdateBackgroundJobRequest
{
    public BackgroundJobDto BackgroundJob { get; set; } = default!;
    public bool IsStartNow { get; set; }
}