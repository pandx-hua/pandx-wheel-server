namespace pandx.Wheel.Models;

public class ListResponse<T>
{
    public ListResponse()
    {
    }

    public ListResponse(IReadOnlyList<T> items)
    {
        Items = items;
    }

    public IReadOnlyList<T> Items { get; set; } = default!;
}