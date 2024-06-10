using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Models;
using pandx.Wheel.Validation;
using Sample.Domain;

namespace Sample.Application.Authorization.Users.Dto;

public class GetUsersRequest : PagedRequest, ISortedRequest, IFilteredRequest, IShouldNormalize
{
    public GetUsersRequest()
    {
        PageSize = SampleConsts.DefaultPageSize;
        StartTime = new DateTime(1900, 1, 1, 0, 0, 0);
        EndTime = new DateTime(2099, 12, 31, 23, 59, 59);
    }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public List<Gender> Gender { get; set; } =
        [pandx.Wheel.Authorization.Users.Gender.Female, pandx.Wheel.Authorization.Users.Gender.Male];

    public List<bool> IsActive { get; set; } = [true, false];
    public List<bool> IsLockout { get; set; } = [true, false];
    public List<bool> IsWeixin { get; set; } = [true, false];
    public string? Filter { get; set; }

    public void Normalize()
    {
        if (string.IsNullOrWhiteSpace(Sorting))
        {
            Sorting = "User.CreationTime DESC";
        }
        else if (Sorting.Contains("UserName", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("UserName", "User.UserName", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("Name", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("Name", "User.Name", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("CreationTime", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("CreationTime", "User.CreationTime", StringComparison.OrdinalIgnoreCase);
        }

        Filter = Filter?.Trim();
    }

    public string? Sorting { get; set; }
}