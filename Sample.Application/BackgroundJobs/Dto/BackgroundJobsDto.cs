using pandx.Wheel.Application.Dto;
using pandx.Wheel.Domain.Entities;

namespace Sample.Application.BackgroundJobs.Dto;

public class BackgroundJobsDto : EntityDto<Guid>, IHasCreationTime
{
    public string JobName { get; set; } = default!;

    public string CronExpression { get; set; } = default!;
    public string? Description { get; set; }

    public string Job { get; set; } = default!;

    public long SuccessCount { get; set; }
    public long FailCount { get; set; }
    public DateTime? NextRunTime { get; set; }
    public DateTime? LastRunTime { get; set; }
    public DateTime? PrevRunTime { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreationTime { get; set; }
}