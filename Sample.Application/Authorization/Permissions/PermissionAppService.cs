using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Permissions.Dto;

namespace Sample.Application.Authorization.Permissions;

public class PermissionAppService : SampleAppServiceBase, IPermissionAppService
{
    private readonly IPermissionManager _permissionManager;

    public PermissionAppService(IPermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    public ListResponse<PermissionWithLevelDto> GetPermissions()
    {
        var permissions = _permissionManager.GetExpandedPermissions();
        var rootPermissions = permissions.Where(p => p.Parent == null);
        var result = new List<PermissionWithLevelDto>();
        foreach (var rootPermission in rootPermissions)
        {
            var level = 0;
            AddPermission(rootPermission, permissions, result, level);
        }

        return new ListResponse<PermissionWithLevelDto>
        {
            Items = result
        };
    }

    private void AddPermission(Permission permission, IReadOnlyList<Permission> permissions,
        List<PermissionWithLevelDto> result, int level)
    {
        var p = Mapper.Map<PermissionWithLevelDto>(permission);
        p.Level = level;
        p.Leaf = permission.Children.Count == 0;
        p.Value = $"Permission.{permission.Resource}.{permission.Action}";
        result.Add(p);

        var children = permissions.Where(p =>
                p.Parent != null && p.Parent.Resource == permission.Resource && p.Parent.Action == permission.Action)
            .ToList();
        foreach (var child in children)
        {
            AddPermission(child, permissions, result, level + 1);
        }
    }
}