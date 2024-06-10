using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Controllers;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Permissions;
using Sample.Application.Authorization.Permissions.Dto;

namespace Sample.Host.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PermissionsController : WheelControllerBase
{
    private readonly IPermissionAppService _permissionAppService;

    public PermissionsController(IPermissionAppService permissionAppService)
    {
        _permissionAppService = permissionAppService;
    }

    [HttpPost(Name = nameof(GetPermissions))]
    [NeedPermission]
    public Task<ListResponse<PermissionWithLevelDto>> GetPermissions()
    {
        return Task.FromResult(_permissionAppService.GetPermissions());
    }
}