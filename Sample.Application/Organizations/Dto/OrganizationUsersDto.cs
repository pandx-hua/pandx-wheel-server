using pandx.Wheel.Application.Dto;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Domain.Entities;
using Sample.Application.Authorization.Users.Dto;

namespace Sample.Application.Organizations.Dto;

public class OrganizationUsersDto : EntityDto<Guid>, IHasCreationTime
{
    public string Name { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public List<UserRolesDto> Roles { get; set; } = default!;
    public List<UserOrganizationsDto> Organizations { get; set; } = default!;
    public Gender Gender { get; set; }
    public bool IsLockout { get; set; }
    public bool IsWeixin { get; set; }
    public bool IsActive { get; set; }
    public DateTime? AddedTime { get; set; }
    public DateTime CreationTime { get; set; }
}