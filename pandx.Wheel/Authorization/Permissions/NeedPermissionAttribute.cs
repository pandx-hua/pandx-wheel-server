using Microsoft.AspNetCore.Authorization;

namespace pandx.Wheel.Authorization.Permissions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class NeedPermissionAttribute : AuthorizeAttribute
{
    public NeedPermissionAttribute()
    {
    }

    public NeedPermissionAttribute(string resource, string action)
    {
        Policy = $"{WheelClaimTypes.Permission}.{resource}.{action}";
    }
}