using System.Collections.Immutable;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Extensions;

namespace pandx.Wheel.Authorization.Permissions;

public class PermissionManager : IPermissionManager
{
    private readonly IPermissionProvider _permissionProvider;
    private readonly IDictionary<string, Permission> _permissions;

    public PermissionManager(IPermissionProvider permissionProvider)
    {
        _permissions = new Dictionary<string, Permission>();
        _permissionProvider = permissionProvider;
    }

    public async Task InitializeAsync()
    {
        var context = new PermissionContext(this);
        await _permissionProvider.SetPermissionsAsync(context);
    }

    public Permission CreatePermission(string resource, string action, string? displayName = null,
        string? description = null, Dictionary<string, object>? properties = null)
    {
        if (_permissions.ContainsKey($"{resource}.{action}"))
        {
            throw new WheelException($"已经存在名称为{resource}.{action}的Permission");
        }

        var permission = new Permission(resource, action, displayName, description, properties);
        _permissions[$"{resource}.{action}"] = permission;
        return permission;
    }

    public Permission? GetPermission(string resource, string action)
    {
        return _permissions.GetOrDefault($"{resource}.{action}");
    }

    public IReadOnlyList<Permission> GetExpandedPermissions()
    {
        var expandedPermissions = new Dictionary<string, Permission>();
        foreach (var permission in _permissions.Values.ToList())
        {
            AddPermissionRecursively(permission, expandedPermissions);
        }

        return expandedPermissions.Values.ToImmutableList();
    }

    public IReadOnlyList<Permission> GetShrunkPermissions()
    {
        return _permissions.Values.ToImmutableList();
    }

    private void AddPermissionRecursively(Permission permission, Dictionary<string, Permission> decrypter)
    {
        if (decrypter.TryGetValue($"{permission.Resource}.{permission.Action}", out var existingPermission))
        {
            if (existingPermission != permission)
            {
                throw new WheelException($"已经存在名称为{permission.Resource}.{permission.Action}的Permission");
            }
        }
        else
        {
            decrypter[$"{permission.Resource}.{permission.Action}"] = permission;
        }

        foreach (var child in permission.Children)
        {
            AddPermissionRecursively(child, decrypter);
        }
    }
}