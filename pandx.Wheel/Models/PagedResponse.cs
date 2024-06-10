namespace pandx.Wheel.Models;

public class PagedResponse<T> : ListResponse<T>
{
    public PagedResponse()
    {
    }

    public PagedResponse(int totalCount, IReadOnlyList<T> items) : base(items)
    {
        TotalCount = totalCount;
    }

    public int TotalCount { get; set; }
}