using pandx.Wheel.Authorization.Roles;
using pandx.Wheel.Domain.Repositories;
using Sample.Domain.Authorization.Users;

namespace Sample.Domain.Authorization.Roles;

public class RoleService : WheelRoleService<ApplicationRole, ApplicationUser>
{
    public RoleService(IRepository<ApplicationRole, Guid> roleRepository) : base(roleRepository)
    {
    }
}