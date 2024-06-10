using pandx.Wheel.Models;
using pandx.Wheel.Notifications;

namespace Sample.Application.Notifications.Dto;

public class GetUserNotificationsRequest : PagedRequest
{
    public UserNotificationState? State { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}