namespace pandx.Wheel.Events;

public interface IEvent
{
    DateTime EventTime { get; set; }
    object EventSource { get; set; }
}