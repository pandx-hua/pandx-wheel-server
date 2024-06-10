using pandx.Wheel.Application.Services;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Permissions.Dto;

namespace Sample.Application.Authorization.Permissions;

public interface IPermissionAppService : IApplicationService
{
    ListResponse<PermissionWithLevelDto> GetPermissions();
}