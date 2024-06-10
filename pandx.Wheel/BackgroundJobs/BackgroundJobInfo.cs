using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.BackgroundJobs;

public class BackgroundJobInfo : AuditedEntity<Guid>
{
    [StringLength(256)] public string JobName { get; set; } = default!;

    [StringLength(256)] public string CronExpression { get; set; } = default!;

    [StringLength(512)] public string? Description { get; set; }

    [StringLength(256)] public string Job { get; set; } = default!;

    public long SuccessCount { get; set; }
    public long FailCount { get; set; }
    public DateTime? NextRunTime { get; set; }
    public DateTime? LastRunTime { get; set; }
    public DateTime? PrevRunTime { get; set; }
    public bool Status { get; set; }
}