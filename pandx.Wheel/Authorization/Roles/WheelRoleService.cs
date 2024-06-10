using Microsoft.AspNetCore.Identity;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Domain.Repositories;

namespace pandx.Wheel.Authorization.Roles;

public abstract class WheelRoleService<TRole, TUser> : ServiceBase, IRoleService<TRole>
    where TRole : WheelRole
    where TUser : WheelUser
{
    private readonly IRepository<TRole, Guid> _roleRepository;

    protected WheelRoleService(IRepository<TRole, Guid> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [Injection] public UserManager<TUser> UserManager { get; set; }

    [Injection] public RoleManager<TRole> RoleManager { get; set; }

    public async Task<List<TRole>> GetStaticRolesAsync()
    {
        return await _roleRepository.GetAllListAsync(r => r.IsDefault);
    }
}