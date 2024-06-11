using pandx.Wheel.Events;

namespace Sample.Application.Common;

public class CommonEventHandler : NotificationEventHandler<CommonEvent>
{
    protected override Task Handle(CommonEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine(@event.Name);
        return Task.CompletedTask;
    }
}