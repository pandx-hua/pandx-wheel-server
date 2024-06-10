using pandx.Wheel.Application.Dto;
using pandx.Wheel.Application.Services;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Users.Dto;
using Sample.Application.Organizations.Dto;

namespace Sample.Application.Organizations;

public interface IOrganizationAppService : IApplicationService
{
    Task<ListResponse<OrganizationDto>> GetOrganizationsAsync();
    Task<PagedResponse<OrganizationUsersDto>> GetOrganizationUsersAsync(GetOrganizationUsersRequest1 request);
    Task<PagedResponse<OrganizationUsersDto>> GetOrganizationUsersAsync(GetOrganizationUsersRequest2 request);
    Task<OrganizationDto> CreateOrganizationAsync(CreateOrganizationRequest request);
    Task<OrganizationDto> UpdateOrganizationAsync(UpdateOrganizationRequest request);
    Task<OrganizationDto> MoveOrganizationAsync(MoveOrganizationRequest request);
    Task DeleteOrganizationAsync(EntityDto<Guid> request);
    Task RemoveUserFromOrganizationAsync(UserToOrganizationRequest request);
    Task RemoveUsersFromOrganizationAsync(UsersToOrganizationRequest request);
    Task AddUsersToOrganizationAsync(UsersToOrganizationRequest request);
    Task<PagedResponse<UsersDto>> FindUsersAsync(FindOrganizationUsersRequest request);
    Task<ValidationResponse> ValidateOrganizationAsync(ValidationRequest<string, string> request);
    Task UpAsync(EntityDto<Guid> request);
    Task DownAsync(EntityDto<Guid> request);
}