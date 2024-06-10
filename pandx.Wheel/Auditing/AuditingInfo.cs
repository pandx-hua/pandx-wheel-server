using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Auditing;

public class AuditingInfo : Entity<Guid>
{
    public Guid? UserId { get; init; }

    [StringLength(256)] public string? Controller { get; set; }

    [StringLength(256)] public string? Action { get; set; }

    [StringLength(5120)] public string? Parameters { get; set; }

    [StringLength(10240)] public string? ReturnValue { get; set; }

    public DateTime ExecutionTime { get; init; }
    public int ExecutionDuration { get; set; }

    [StringLength(256)] public string? ClientIpAddress { get; set; }

    [StringLength(512)] public string? ClientName { get; set; }

    [StringLength(512)] public string? BrowserInfo { get; set; }

    [StringLength(10240)] public string? Exception { get; set; }
}