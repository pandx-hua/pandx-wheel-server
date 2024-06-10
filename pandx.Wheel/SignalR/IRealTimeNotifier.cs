using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Notifications;

namespace pandx.Wheel.SignalR;

public interface IRealTimeNotifier : ITransientDependency
{
    Task SendNotificationsAsync(UserNotification[] userNotifications);
}