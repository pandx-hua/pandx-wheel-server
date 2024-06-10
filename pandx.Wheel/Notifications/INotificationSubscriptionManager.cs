using pandx.Wheel.Authorization;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Notifications;

public interface INotificationSubscriptionManager : ITransientDependency
{
    Task SubscribeAsync(UserIdentifier userIdentifier, string notificationName);
    Task<bool> IsSubscribedAsync(UserIdentifier userIdentifier, string notificationName);
    Task SubscribeToAllNotificationsAsync(UserIdentifier userIdentifier);
}