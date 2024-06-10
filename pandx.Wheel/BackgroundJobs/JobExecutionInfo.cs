using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.BackgroundJobs;

public class JobExecutionInfo : Entity<Guid>
{
    public Guid BackgroundJobId { get; set; }

    [StringLength(256)] public string JobName { get; set; } = default!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Duration { get; set; }
    public bool Result { get; set; }

    [StringLength(10240)] public string? Exception { get; set; }
}