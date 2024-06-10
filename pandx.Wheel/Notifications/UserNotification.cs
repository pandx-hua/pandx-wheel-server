using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Authorization;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Notifications;

public class UserNotification : AuditedEntity<Guid>, IUserIdentifier
{
    [StringLength(256)] public string NotificationName { get; set; } = default!;

    [StringLength(5120)] public string? NotificationData { get; set; }

    public NotificationSeverity Severity { get; set; }

    public UserNotificationState State { get; set; }
    public Guid UserId { get; set; }
}

public enum UserNotificationState
{
    Unread = 0,
    Read = 1
}