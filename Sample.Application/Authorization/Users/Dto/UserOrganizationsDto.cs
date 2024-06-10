using pandx.Wheel.Application.Dto;

namespace Sample.Application.Authorization.Users.Dto;

public class UserOrganizationsDto : EntityDto<Guid>
{
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}