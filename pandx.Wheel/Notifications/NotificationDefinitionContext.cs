namespace pandx.Wheel.Notifications;

public class NotificationDefinitionContext : INotificationDefinitionContext
{
    public NotificationDefinitionContext(INotificationDefinitionManager manager)
    {
        Manager = manager;
    }

    public INotificationDefinitionManager Manager { get; }
}