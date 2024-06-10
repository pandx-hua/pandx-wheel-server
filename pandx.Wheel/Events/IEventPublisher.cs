using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Events;

public interface IEventPublisher : ITransientDependency
{
    Task PublishAsync(IEvent @event);
}