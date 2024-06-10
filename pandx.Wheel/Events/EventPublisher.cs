using MediatR;
using Microsoft.Extensions.Logging;

namespace pandx.Wheel.Events;

//此处的事件发布不在Infrastructure实现，因为已经和MediatR深度耦合，放在Infrastructure实现已经没有意义了
public class EventPublisher : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger;
    private readonly IMediator _mediator;
    private readonly IPublisher _publisher;

    public EventPublisher(ILogger<EventPublisher> logger, IPublisher publisher, IMediator mediator)
    {
        _logger = logger;
        _publisher = publisher;
        _mediator = mediator;
    }

    public Task PublishAsync(IEvent @event)
    {
        _logger.LogInformation("触发了事件 {event}", @event);
        return _mediator.Publish(CreateEventNotification(@event));
    }

    private static INotification CreateEventNotification(IEvent @event)
    {
        return (INotification)Activator.CreateInstance(typeof(EventNotification<>).MakeGenericType(@event.GetType()),
            @event)!;
    }
}