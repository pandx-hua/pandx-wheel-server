using pandx.Wheel.Events;

namespace Sample.Application.Common;

public class CommonEvent : Event
{
    public string Name { get; set; } = default!;
}