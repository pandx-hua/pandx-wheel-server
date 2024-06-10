using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Notifications;

public interface INotificationPublisher : ITransientDependency
{
    Task PublishAsync(string notificationName, NotificationData? data = null,
        NotificationSeverity severity = NotificationSeverity.Info, Guid[]? userIds = null,
        Guid[]? excludedUserIds = null);
}