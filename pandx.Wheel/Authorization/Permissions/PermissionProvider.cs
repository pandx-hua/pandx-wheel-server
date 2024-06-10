namespace pandx.Wheel.Authorization.Permissions;

public abstract class PermissionProvider : IPermissionProvider
{
    public abstract Task SetPermissionsAsync(IPermissionContext context);
}