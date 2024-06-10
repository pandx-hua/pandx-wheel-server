using MediatR;

namespace pandx.Wheel.Events;

public interface INotificationEventHandler<TEvent> : INotificationHandler<EventNotification<TEvent>>
    where TEvent : IEvent
{
}