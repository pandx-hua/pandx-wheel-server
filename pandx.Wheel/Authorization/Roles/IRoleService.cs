using Microsoft.AspNetCore.Identity;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Authorization.Roles;

public interface IRoleService<TRole> : ITransientDependency where TRole : WheelRole
{
    RoleManager<TRole> RoleManager { get; set; }
    Task<List<TRole>> GetStaticRolesAsync();
}