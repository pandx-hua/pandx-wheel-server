using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Controllers;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Roles;
using Sample.Application.Authorization.Roles.Dto;
using Sample.Domain.Authorization.Permissions;

namespace Sample.Host.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class RolesController : WheelControllerBase
{
    private readonly IRoleAppService _roleAppService;

    public RolesController(IRoleAppService roleAppService)
    {
        _roleAppService = roleAppService;
    }

    [NeedPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Create)]
    [NeedPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Update)]
    [HttpPost(Name = nameof(CreateOrUpdateRole))]
    public async Task CreateOrUpdateRole(CreateOrUpdateRoleRequest request)
    {
        await _roleAppService.CreateOrUpdateRoleAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Search)]
    [HttpPost(Name = nameof(GetPagedRoles))]
    public async Task<PagedResponse<RolesDto>> GetPagedRoles(GetRolesRequest request)
    {
        return await _roleAppService.GetPagedRolesAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Search)]
    [HttpPost(Name = nameof(GetRoles))]
    public async Task<ListResponse<RolesDto>> GetRoles()
    {
        return await _roleAppService.GetRolesAsync();
    }

    [NeedPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Delete)]
    [HttpPost(Name = nameof(DeleteRole))]
    public async Task DeleteRole(EntityDto<Guid> request)
    {
        await _roleAppService.DeleteRoleAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Delete)]
    [HttpPost(Name = nameof(BatchDeleteRoles))]
    public async Task<int> BatchDeleteRoles(BatchDeleteRolesRequest request)
    {
        return await _roleAppService.BatchDeleteRolesAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Create)]
    [NeedPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Update)]
    [HttpPost(Name = nameof(ValidateRoleName))]
    [NoAudited]
    public async Task<ValidationResponse> ValidateRoleName(ValidationRequest<string, string> request)
    {
        return await _roleAppService.ValidateRoleNameAsync(request);
    }
}