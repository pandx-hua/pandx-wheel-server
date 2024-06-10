namespace pandx.Wheel.Authorization.Permissions;

public class PermissionContext : IPermissionContext
{
    public PermissionContext(IPermissionManager manager)
    {
        Manager = manager;
    }

    public IPermissionManager Manager { get; }
}