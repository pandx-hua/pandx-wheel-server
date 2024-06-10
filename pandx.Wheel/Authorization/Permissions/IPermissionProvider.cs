using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Authorization.Permissions;

public interface IPermissionProvider : ITransientDependency
{
    Task SetPermissionsAsync(IPermissionContext context);
}