using pandx.Wheel.Notifications;

namespace Sample.Application.Notifications.Dto;

public class GetUserNotificationCountRequest
{
    public Guid UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public UserNotificationState State { get; set; }
}