using pandx.Wheel.Application.Dto;
using pandx.Wheel.Application.Services;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Roles.Dto;

namespace Sample.Application.Authorization.Roles;

public interface IRoleAppService : IApplicationService
{
    Task<ListResponse<RolesDto>> GetRolesAsync();
    Task<PagedResponse<RolesDto>> GetPagedRolesAsync(GetRolesRequest request);
    Task DeleteRoleAsync(EntityDto<Guid> request);
    Task CreateOrUpdateRoleAsync(CreateOrUpdateRoleRequest request);
    Task<int> BatchDeleteRolesAsync(BatchDeleteRolesRequest request);
    Task<ValidationResponse> ValidateRoleNameAsync(ValidationRequest<string, string> request);
}