using pandx.Wheel.Application.Dto;

namespace Sample.Application.Authorization.Users.Dto;

public class UserRolesDto : EntityDto<Guid>
{
    public string DisplayName { get; set; } = default!;
}