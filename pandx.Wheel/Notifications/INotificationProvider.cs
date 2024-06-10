using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Notifications;

public interface INotificationProvider : ITransientDependency
{
    Task SetNotificationsAsync(INotificationDefinitionContext context);
}