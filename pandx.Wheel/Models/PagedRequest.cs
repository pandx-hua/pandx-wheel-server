namespace pandx.Wheel.Models;

public class PagedRequest
{
    protected PagedRequest()
    {
        CurrentPage = 1;
        PageSize = 10;
    }

    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}