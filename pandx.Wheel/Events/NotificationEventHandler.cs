namespace pandx.Wheel.Events;

public abstract class NotificationEventHandler<TEvent> : INotificationEventHandler<TEvent> where TEvent : IEvent
{
    public Task Handle(EventNotification<TEvent> notification, CancellationToken cancellationToken)
    {
        return Handle(notification.Event, cancellationToken);
    }

    protected abstract Task Handle(TEvent @event, CancellationToken cancellationToken);
}