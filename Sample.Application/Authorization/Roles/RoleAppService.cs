using System.Linq.Dynamic.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Authorization;
using pandx.Wheel.Authorization.Roles;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Events;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Extensions;
using pandx.Wheel.Helpers;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Roles.Dto;
using Sample.Domain.Authorization.Roles;
using Sample.Domain.Authorization.Users;

namespace Sample.Application.Authorization.Roles;

public class RoleAppService : SampleAppServiceBase, IRoleAppService
{
    private readonly IEventPublisher _eventPublisher;

    private readonly IUnitOfWork _unitOfWork;

    public RoleAppService(IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task<ListResponse<RolesDto>> GetRolesAsync()
    {
        var query = RoleService.RoleManager.Roles;
        var roles = await query.OrderBy(x => x.DisplayName).ToListAsync();
        return new ListResponse<RolesDto>(Mapper.Map<List<RolesDto>>(roles));
    }

    public async Task<PagedResponse<RolesDto>> GetPagedRolesAsync(GetRolesRequest request)
    {
        var query = RoleService.RoleManager.Roles
            .Where(x => x.CreationTime >= request.StartTime && x.CreationTime <= request.EndTime)
            .WhereIf(request.IsStatic.Count > 0, x => request.IsStatic.Contains(x.IsStatic))
            .WhereIf(request.IsDefault.Count > 0, x => request.IsDefault.Contains(x.IsDefault))
            .WhereIf(!string.IsNullOrWhiteSpace(request.Filter),
                x => x.DisplayName!.Contains(request.Filter!) || x.Name!.Contains(request.Filter!) ||
                     x.Description!.Contains(request.Filter!));
        var totalCount = await query.CountAsync();
        var roles = await query.OrderBy(request.Sorting!).PageBy(request).ToListAsync();
        var dtos = roles.Select(x =>
        {
            var dto = Mapper.Map<RolesDto>(x);
            dto.Users = Mapper.Map<List<RoleUsersDto>>(AsyncHelper.RunSync(() =>
                UserService.UserManager.GetUsersInRoleAsync(x.Name!)));
            dto.Permissions =
                Mapper.Map<List<RoleClaimsDto>>(AsyncHelper.RunSync(() => RoleService.RoleManager.GetClaimsAsync(x)));
            return dto;
        }).ToList();
        return new PagedResponse<RolesDto>(totalCount, dtos);
    }

    public async Task DeleteRoleAsync(EntityDto<Guid> request)
    {
        var role = await RoleService.RoleManager.FindByIdAsync(request.Id.ToString()) ??
                   throw new Exception("没有发现指定的角色");
        if (role.IsStatic)
        {
            throw new WheelException("无法删除系统角色");
        }

        var users = await UserService.UserManager.GetUsersInRoleAsync(role.Name!);
        foreach (var user in users)
        {
            CheckErrors(await UserService.UserManager.RemoveFromRoleAsync(user, role.Name!));
        }


        CheckErrors(await RoleService.RoleManager.DeleteAsync(role));
        await _unitOfWork.CommitAsync();
        await _eventPublisher.PublishAsync(new RoleDeletedEvent<ApplicationRole>(role));
    }

    public async Task<int> BatchDeleteRolesAsync(BatchDeleteRolesRequest request)
    {
        var i = 0;
        foreach (var roleId in request.RoleIds)
        {
            try
            {
                await DeleteRoleAsync(new EntityDto<Guid> { Id = roleId });
            }
            catch
            {
                continue;
            }

            i++;
        }

        return i;
    }

    public async Task<ValidationResponse> ValidateRoleNameAsync(ValidationRequest<string, string> request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            var one = await RoleService.RoleManager.Roles.IgnoreQueryFilters()
                .SingleOrDefaultAsync(r => r.Name == request.Value);
            if (one != null)
            {
                return new ValidationResponse
                {
                    Status = false,
                    Message = "角色名称 " + request.Value + " 已被占用"
                };
            }

            return new ValidationResponse
            {
                Status = true
            };
        }
        else
        {
            var one = await RoleService.RoleManager.Roles.IgnoreQueryFilters().SingleOrDefaultAsync(r =>
                r.Name == request.Value && r.Id.ToString() != request.Id);
            if (one != null)
            {
                return new ValidationResponse
                {
                    Status = false,
                    Message = "角色名称 " + request.Value + " 已被占用"
                };
            }

            return new ValidationResponse
            {
                Status = true
            };
        }
    }


    public async Task CreateOrUpdateRoleAsync(CreateOrUpdateRoleRequest request)
    {
        if (request.Role.Id is not null)
        {
            await UpdateRoleAsync(request);
        }
        else
        {
            await CreateRoleAsync(request);
        }
    }

    private async Task CreateRoleAsync(CreateOrUpdateRoleRequest request)
    {
        var role = new ApplicationRole
        {
            IsDefault = request.Role.IsDefault,
            Description = request.Role.Description,
            Name = request.Role.DisplayName,
            DisplayName = request.Role.DisplayName
        };
        CheckErrors(await RoleService.RoleManager.CreateAsync(role));
        await _unitOfWork.CommitAsync();
        await UpdateMembersAsync(role, request.UserIds);
        await UpdateGrantedPermissionsAsync(role, request.GrantedPermissions);
        await _eventPublisher.PublishAsync(new RoleCreatedEvent<ApplicationRole>(role));
    }

    private async Task UpdateMembersAsync(ApplicationRole role, List<string> userIds)
    {
        var users1 = await UserService.UserManager.GetUsersInRoleAsync(role.Name!);
        var users2 = new List<ApplicationUser>();
        foreach (var userId in userIds)
        {
            var user = await UserService.UserManager.FindByIdAsync(userId);
            if (user is not null)
            {
                users2.Add(user);
            }
        }

        var exceptUsers1 = users2.Except(users1);
        foreach (var user in exceptUsers1)
        {
            CheckErrors(await UserService.UserManager.AddToRoleAsync(user, role.Name!));
        }

        var exceptUsers2 = users1.Except(users2);
        foreach (var user in exceptUsers2)
        {
            CheckErrors(await UserService.UserManager.RemoveFromRoleAsync(user, role.Name!));
        }
    }

    private async Task UpdateGrantedPermissionsAsync(ApplicationRole role, List<string> grantedPermissions)
    {
        var currentClaims = await RoleService.RoleManager.GetClaimsAsync(role);
        foreach (var claim in currentClaims.Where(c => grantedPermissions.All(p => p != c.Value)))
        {
            CheckErrors(await RoleService.RoleManager.RemoveClaimAsync(role, claim));
        }

        foreach (var permission in grantedPermissions.Where(c => currentClaims.All(p => p.Value != c)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                CheckErrors(await RoleService.RoleManager.AddClaimAsync(role,
                    new Claim(WheelClaimTypes.Permission, permission)));
            }
        }
    }

    private void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors();
    }

    private async Task UpdateRoleAsync(CreateOrUpdateRoleRequest request)
    {
        var role = await RoleService.RoleManager.FindByIdAsync(request.Role.Id!) ?? throw new Exception("没有发现指定的角色");

        role.DisplayName = request.Role.DisplayName;
        role.Description = request.Role.Description;
        role.Name = request.Role.DisplayName;
        role.IsDefault = request.Role.IsDefault;

        await UpdateMembersAsync(role, request.UserIds);
        await UpdateGrantedPermissionsAsync(role, request.GrantedPermissions);
        await _eventPublisher.PublishAsync(new RoleUpdatedEvent<ApplicationRole>(role));
    }
}