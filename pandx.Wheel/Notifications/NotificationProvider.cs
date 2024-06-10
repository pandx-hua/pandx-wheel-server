namespace pandx.Wheel.Notifications;

public abstract class NotificationProvider : INotificationProvider
{
    public abstract Task SetNotificationsAsync(INotificationDefinitionContext context);
}