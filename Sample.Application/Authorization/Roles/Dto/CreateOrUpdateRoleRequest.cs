namespace Sample.Application.Authorization.Roles.Dto;

public class CreateOrUpdateRoleRequest
{
    public RoleDto Role { get; set; } = default!;
    public List<string> GrantedPermissions { get; set; } = default!;
    public List<string> UserIds { get; set; } = default!;
}