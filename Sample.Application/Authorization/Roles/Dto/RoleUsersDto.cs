using pandx.Wheel.Application.Dto;

namespace Sample.Application.Authorization.Roles.Dto;

public class RoleUsersDto : EntityDto<Guid>
{
    public string UserName { get; set; } = default!;
    public string Name { get; set; } = default!;
}