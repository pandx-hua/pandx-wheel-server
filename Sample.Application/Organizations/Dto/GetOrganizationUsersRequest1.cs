using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Models;
using pandx.Wheel.Validation;

namespace Sample.Application.Organizations.Dto;

public class GetOrganizationUsersRequest1 : PagedRequest, ISortedRequest, IFilteredRequest, IShouldNormalize
{
    public Guid OrganizationId { get; set; }

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
            Sorting = "UserOrganization.CreationTime DESC";
        }
        else if (Sorting.Contains("UserName", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("UserName", "User.UserName", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("Name", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("Name", "User.Name", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("AddedTime", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("AddedTime", "UserOrganization.CreationTime", StringComparison.OrdinalIgnoreCase);
        }

        Filter = Filter?.Trim();
    }

    public string? Sorting { get; set; }
}