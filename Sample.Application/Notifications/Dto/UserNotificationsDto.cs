using pandx.Wheel.Application.Dto;
using pandx.Wheel.Domain.Entities;
using pandx.Wheel.Notifications;

namespace Sample.Application.Notifications.Dto;

public class UserNotificationsDto : EntityDto<Guid>, IHasCreationTime
{
    public string NotificationName { get; set; } = default!;
    public string? NotificationData { get; set; }
    public NotificationSeverity Severity { get; set; }
    public UserNotificationState State { get; set; }
    public DateTime CreationTime { get; set; }
}