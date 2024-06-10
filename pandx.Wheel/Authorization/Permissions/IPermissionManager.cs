using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Authorization.Permissions;

public interface IPermissionManager : ISingletonDependency
{
    Task InitializeAsync();
    Permission? GetPermission(string resource, string action);

    Permission CreatePermission(string resource, string action, string? displayName = null,
        string? description = null, Dictionary<string, object>? properties = null);

    IReadOnlyList<Permission> GetExpandedPermissions();
    IReadOnlyList<Permission> GetShrunkPermissions();
}