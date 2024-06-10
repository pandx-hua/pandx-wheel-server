using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Notifications;

public class Notification : AuditedEntity<Guid>
{
    [StringLength(256)] public string NotificationName { get; set; } = default!;

    [StringLength(5120)] public string? NotificationData { get; set; }

    public NotificationSeverity Severity { get; set; }

    [StringLength(512)] public string? UserIds { get; set; }

    [StringLength(512)] public string? ExcludedUserIds { get; set; }
}

public enum NotificationSeverity
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
    Fetal = 4
}