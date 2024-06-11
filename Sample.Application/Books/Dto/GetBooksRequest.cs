using pandx.Wheel.Models;
using pandx.Wheel.Validation;
using Sample.Domain;

namespace Sample.Application.Books.Dto;

public class GetBooksRequest : PagedRequest, ISortedRequest, IFilteredRequest, IShouldNormalize
{
    public GetBooksRequest()
    {
        PageSize = SampleConsts.DefaultPageSize;
        StartTime = new DateTime(1900, 1, 1, 0, 0, 0);
        EndTime = new DateTime(2099, 12, 31, 23, 59, 59);
    }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Filter { get; set; }

    public void Normalize()
    {
        if (string.IsNullOrWhiteSpace(Sorting))
        {
            Sorting = "CreationTime DESC";
        }

        Filter = Filter?.Trim();
    }

    public string? Sorting { get; set; }
}