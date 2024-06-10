using System.Collections.Immutable;
using pandx.Wheel.Extensions;

namespace pandx.Wheel.Authorization.Permissions;

public class Permission
{
    private readonly List<Permission> _children;

    public Permission(string resource, string action, string? displayName = null, string? description = null,
        Dictionary<string, object>? properties = null)
    {
        if (resource is null || action is null)
        {
            throw new ArgumentException($"{nameof(resource)}.{nameof(action)}");
        }

        Parent = null;
        Resource = resource;
        Action = action;
        DisplayName = displayName ?? "";
        Description = description ?? "";
        Properties = properties ?? new Dictionary<string, object>();
        _children = new List<Permission>();
    }

    public Permission? Parent { get; private set; }
    public string Resource { get; set; }
    public string Action { get; set; }

    public string DisplayName { get; set; }
    public string Description { get; set; }

    public IDictionary<string, object> Properties { get; set; }

    public object this[string key]
    {
        get => Properties.GetOrDefault(key);
        set => Properties[key] = value;
    }

    public IReadOnlyList<Permission> Children => _children.ToImmutableList();

    public Permission CreateChildPermission(string resource, string action, string? displayName = null,
        string? description = null, Dictionary<string, object>? properties = null)
    {
        var permission = new Permission(resource, action, displayName, description, properties) { Parent = this };
        _children.Add(permission);
        return permission;
    }

    public void RemoveChildPermission(string resource, string action)
    {
        _children.RemoveAll(p => p.Resource == resource && p.Action == action);
    }

    public override string ToString()
    {
        return $"[Permission: {Resource}.{Action}]";
    }
}