using pandx.Wheel.Application.Dto;
using pandx.Wheel.Domain.Entities;

namespace Sample.Application.Authorization.Roles.Dto;

public class RolesDto : EntityDto<Guid>, IHasCreationTime
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool IsStatic { get; set; }
    public bool IsDefault { get; set; }
    public List<RoleUsersDto> Users { get; set; } = default!;
    public List<RoleClaimsDto> Permissions { get; set; } = default!;
    public DateTime CreationTime { get; set; }
}