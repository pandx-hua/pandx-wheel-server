using pandx.Wheel.Domain.Services;
using pandx.Wheel.Notifications;

namespace Sample.Domain.Notifications;

public interface ISampleNotifier : IDomainService
{
    Task SendInvalidExcelNotification(NotificationData data, Guid[] userIds);
    Task SendInvalidUsersNotification(NotificationData data, Guid[] userIds);
    Task SendCommonMessageNotification(NotificationData data, NotificationSeverity severity, Guid[] userIds);
}