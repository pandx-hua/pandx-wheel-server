using pandx.Wheel.Validation;

namespace Sample.Application.Authorization.Roles.Dto;

public class CreateOrUpdateRoleRequest:IShouldValidate
{
    public RoleDto Role { get; set; } = default!;
    public List<string> GrantedPermissions { get; set; } = default!;
    public List<string> UserIds { get; set; } = default!;
}

public class CreateOrUpdateRoleRequestValidator : CustomValidator<CreateOrUpdateRoleRequest>
{
    public CreateOrUpdateRoleRequestValidator()
    {
        
    }
}