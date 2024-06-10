namespace pandx.Wheel.Models;

public class ValidationRequest<T1, T2>
{
    public T1 Value { get; set; } = default!;
    public T2 Id { get; set; } = default!;
    public T2? ParentId { get; set; }
}