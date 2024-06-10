using Microsoft.AspNetCore.Identity;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Organizations;

namespace pandx.Wheel.Authorization.Users;

public interface IUserService<TUser> : ITransientDependency where TUser : WheelUser
{
    UserManager<TUser> UserManager { get; set; }
    SignInManager<TUser> SignInManager { get; set; }
    Task<bool> IsInOrganizationAsync(Guid userId, Guid organizationId);
    Task<bool> IsInOrganizationAsync(TUser user, Organization organization);
    Task AddToOrganizationAsync(Guid userId, Guid organizationId);
    Task AddToOrganizationAsync(TUser user, Organization organization);
    Task RemoveFromOrganizationAsync(Guid userId, Guid organizationId);
    Task RemoveFromOrganizationAsync(TUser user, Organization organization);
    Task RemoveFromOrganizationsAsync(TUser user);
    Task RemoveFromOrganizationsAsync(Guid userId);
    Task<List<TUser>> GetUsersInOrganizationAsync(Organization organization, bool includeChildren = false);
    Task<List<Organization>> GetOrganizationsAsync(TUser user);
    Task SetOrganizationsAsync(Guid userId, params Guid[] organizationIds);
    Task SetOrganizationsAsync(TUser user, params Guid[] organizationIds);
    Task<IdentityResult> SetRolesAsync(TUser user, List<string> assignedRoleNames);
    Task<bool> HasPermissionAsync(Guid userId, string permission);
    Task<List<string>> GetPermissionsAsync(Guid userId);
}