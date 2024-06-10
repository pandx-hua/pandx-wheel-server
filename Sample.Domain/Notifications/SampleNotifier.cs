using pandx.Wheel.Notifications;

namespace Sample.Domain.Notifications;

public class SampleNotifier : SampleDomainServiceBase, ISampleNotifier
{
    private readonly INotificationPublisher _notificationPublisher;

    public SampleNotifier(INotificationPublisher notificationPublisher)
    {
        _notificationPublisher = notificationPublisher;
    }

    public Task SendInvalidExcelNotification(NotificationData data, Guid[] userIds)
    {
        return _notificationPublisher.PublishAsync(SampleNotificationNames.InvalidExcel, data,
            NotificationSeverity.Error,
            userIds);
    }

    public Task SendInvalidUsersNotification(NotificationData data, Guid[] userIds)
    {
        return _notificationPublisher.PublishAsync(SampleNotificationNames.InvalidUsers, data,
            NotificationSeverity.Warning,
            userIds);
    }

    public Task SendCommonMessageNotification(NotificationData data, NotificationSeverity severity, Guid[] userIds)
    {
        return _notificationPublisher.PublishAsync(SampleNotificationNames.CommonMessage, data,
            severity,
            userIds);
    }
}