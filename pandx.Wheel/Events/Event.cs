namespace pandx.Wheel.Events;

public abstract class Event : IEvent
{
    public DateTime EventTime { get; set; } = DateTime.Now;
    public object EventSource { get; set; } = default!;
}