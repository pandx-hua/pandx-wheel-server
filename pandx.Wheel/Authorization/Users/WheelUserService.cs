using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Authorization.Roles;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.Services;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Organizations;

namespace pandx.Wheel.Authorization.Users;

public abstract class WheelUserService<TUser, TRole> : DomainServiceBase, IUserService<TUser>
    where TUser : WheelUser
    where TRole : WheelRole
{
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRepository<UserOrganization> _userOrganizationRepository;

    protected WheelUserService(IRepository<UserOrganization> userOrganizationRepository,
        IRepository<Organization, Guid> organizationRepository,
        IUnitOfWork unitOfWork)
    {
        _userOrganizationRepository = userOrganizationRepository;
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
    }

    [Injection] public RoleManager<TRole> RoleManager { get; set; } = default!;
    [Injection] public UserManager<TUser> UserManager { get; set; } = default!;
    [Injection] public SignInManager<TUser> SignInManager { get; set; } = default!;

    public async Task<bool> IsInOrganizationAsync(Guid userId, Guid organizationId)
    {
        return await IsInOrganizationAsync((await UserManager.FindByIdAsync(userId.ToString()))!,
            await _organizationRepository.GetAsync(organizationId));
    }

    public async Task<bool> IsInOrganizationAsync(TUser user, Organization organization)
    {
        return await _userOrganizationRepository.CountAsync(uo =>
            uo.UserId == user.Id && uo.OrganizationId == organization.Id) > 0;
    }

    public async Task AddToOrganizationAsync(Guid userId, Guid organizationId)
    {
        await AddToOrganizationAsync((await UserManager.FindByIdAsync(userId.ToString()))!,
            await _organizationRepository.GetAsync(organizationId));
    }

    public async Task AddToOrganizationAsync(TUser user, Organization organization)
    {
        var currentOrganizations = await GetOrganizationsAsync(user);
        if (currentOrganizations.Any(o => o.Id == organization.Id))
        {
            return;
        }

        await _userOrganizationRepository.InsertAsync(new UserOrganization
        {
            UserId = user.Id,
            OrganizationId = organization.Id
        });
    }

    public async Task RemoveFromOrganizationAsync(Guid userId, Guid organizationId)
    {
        await RemoveFromOrganizationAsync((await UserManager.FindByIdAsync(userId.ToString()))!,
            await _organizationRepository.GetAsync(organizationId));
    }

    public async Task RemoveFromOrganizationAsync(TUser user, Organization organization)
    {
        await _userOrganizationRepository.DeleteAsync(
            uo => uo.UserId == user.Id && uo.OrganizationId == organization.Id);
    }

    public async Task RemoveFromOrganizationsAsync(TUser user)
    {
        await _userOrganizationRepository.DeleteAsync(
            uo => uo.UserId == user.Id);
    }

    public async Task RemoveFromOrganizationsAsync(Guid userId)
    {
        await RemoveFromOrganizationsAsync((await UserManager.FindByIdAsync(userId.ToString()))!);
    }

    public async Task<List<TUser>> GetUsersInOrganizationAsync(Organization organization, bool includeChildren = false)
    {
        if (!includeChildren)
        {
            var query = from uo in await _userOrganizationRepository.GetAllAsync()
                join user in UserManager.Users on uo.UserId equals user.Id
                where uo.OrganizationId == organization.Id
                select user;
            return await query.ToListAsync();
        }
        else
        {
            var query = from uo in await _userOrganizationRepository.GetAllAsync()
                join user in UserManager.Users on uo.UserId equals user.Id
                join o in await _organizationRepository.GetAllAsync() on uo.OrganizationId equals o.Id
                where o.Code.StartsWith(organization.Code)
                select user;
            return await query.ToListAsync();
        }
    }

    public async Task<List<Organization>> GetOrganizationsAsync(TUser user)
    {
        var query = from uo in await _userOrganizationRepository.GetAllAsync()
            join o in await _organizationRepository.GetAllAsync() on uo.OrganizationId equals o.Id
            where uo.UserId == user.Id
            select o;
        return await query.ToListAsync();
    }

    public async Task SetOrganizationsAsync(Guid userId, params Guid[] organizationIds)
    {
        await SetOrganizationsAsync((await UserManager.FindByIdAsync(userId.ToString()))!, organizationIds);
    }

    public async Task SetOrganizationsAsync(TUser user, params Guid[] organizationIds)
    {
        var currentOrganizations = await GetOrganizationsAsync(user);
        //remove
        foreach (var currentOrganization in currentOrganizations)
        {
            if (!organizationIds.Contains(currentOrganization.Id))
            {
                await RemoveFromOrganizationAsync(user, currentOrganization);
            }
        }

        await _unitOfWork.CommitAsync();
        //add
        foreach (var organizationId in organizationIds)
        {
            if (currentOrganizations.All(o => o.Id != organizationId))
            {
                await AddToOrganizationAsync(user, await _organizationRepository.GetAsync(organizationId));
            }
        }
    }

    public async Task<IdentityResult> SetRolesAsync(TUser user, List<string> assignedRoleNames)
    {
        var userRoleNames = await UserManager.GetRolesAsync(user);
        //remove from removed roles
        foreach (var userRoleName in userRoleNames)
        {
            if (assignedRoleNames.All(an => an != userRoleName))
            {
                var result = await UserManager.RemoveFromRoleAsync(user, userRoleName);
                if (!result.Succeeded)
                {
                    return result;
                }
            }
        }

        //add to added roles
        foreach (var assignedRoleName in assignedRoleNames)
        {
            if (userRoleNames.All(rn => rn != assignedRoleName))
            {
                var result = await UserManager.AddToRoleAsync(user, assignedRoleName);
                if (!result.Succeeded)
                {
                    return result;
                }
            }
        }

        return IdentityResult.Success;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission)
    {
        var permissions = await CacheService.GetOrSetAsync(
            CacheKeyService.GetCacheKey(WheelClaimTypes.Permission, userId.ToString()),
            () => GetPermissionsAsync(userId));
        return permissions?.Contains(permission) ?? false;
        // var permissions = await GetPermissionsAsync(userId);
        // return permissions?.Contains(permission) ?? false;
    }

    public async Task<List<string>> GetPermissionsAsync(Guid userId)
    {
        var user = await UserManager.FindByIdAsync(userId.ToString());
        _ = user ?? throw new Exception("未能找到指定的用户");

        var userRoles = await UserManager.GetRolesAsync(user);
        var permissions = new List<string>();
        foreach (var role in await RoleManager.Roles.Where(r => userRoles.Contains(r.Name!)).ToListAsync())
        {
            permissions.AddRange((await RoleManager.GetClaimsAsync(role))
                .Where(c => c.Type == WheelClaimTypes.Permission).Select(c => c.Value).ToList());
        }

        return permissions.Distinct().ToList();
    }
}