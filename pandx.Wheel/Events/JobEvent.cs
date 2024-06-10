namespace pandx.Wheel.Events;

public class JobEvent<T> : Event
{
    public JobEvent(T data)
    {
        Data = data;
    }

    public T Data { get; private set; }
}