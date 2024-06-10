using pandx.Wheel.Application.Dto;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Domain.Entities;

namespace Sample.Application.Authorization.Users.Dto;

public class UsersDto : EntityDto<Guid>, IHasCreationTime
{
    public string Name { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public List<UserRolesDto> Roles { get; set; } = default!;
    public List<UserOrganizationsDto> Organizations { get; set; } = default!;
    public Gender Gender { get; set; }
    public bool IsLockout { get; set; }
    public bool LockoutEnabled { get; set; }
    public bool IsWeixin { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
}