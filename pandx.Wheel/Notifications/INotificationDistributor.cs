using pandx.Wheel.Authorization;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Notifications;

public interface INotificationDistributor : ITransientDependency
{
    Task DistributeAsync(Guid notificationId);
    Task<UserIdentifier[]> GetUsersAsync(Notification notification);

    Task<List<UserNotification>> SaveUserNotificationsAsync(UserIdentifier[] userIdentifiers,
        Notification notification);

    Task NotifyAsync(UserNotification[] userNotifications);
}