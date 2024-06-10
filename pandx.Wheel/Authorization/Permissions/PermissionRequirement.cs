using Microsoft.AspNetCore.Authorization;

namespace pandx.Wheel.Authorization.Permissions;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; init; }
}