using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Auditing;
using pandx.Wheel.Controllers;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Users.Dto;
using Sample.Application.Organizations;
using Sample.Application.Organizations.Dto;

namespace Sample.Host.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class OrganizationsController : WheelControllerBase
{
    private readonly IOrganizationAppService _organizationAppService;


    public OrganizationsController(IOrganizationAppService organizationAppService)
    {
        _organizationAppService = organizationAppService;
    }


    [HttpPost(Name = nameof(GetOrganizations))]
    public async Task<ListResponse<OrganizationDto>> GetOrganizations()
    {
        return await _organizationAppService.GetOrganizationsAsync();
    }

    [HttpPost(Name = nameof(GetOrganizationUsers1))]
    public async Task<PagedResponse<OrganizationUsersDto>> GetOrganizationUsers1(GetOrganizationUsersRequest1 request)
    {
        return await _organizationAppService.GetOrganizationUsersAsync(request);
    }

    [HttpPost(Name = nameof(GetOrganizationUsers2))]
    public async Task<PagedResponse<OrganizationUsersDto>> GetOrganizationUsers2(GetOrganizationUsersRequest2 request)
    {
        return await _organizationAppService.GetOrganizationUsersAsync(request);
    }

    [HttpPost(Name = nameof(CreateOrganization))]
    public async Task<OrganizationDto> CreateOrganization(CreateOrganizationRequest request)
    {
        return await _organizationAppService.CreateOrganizationAsync(request);
    }

    [HttpPost(Name = nameof(UpdateOrganization))]
    public async Task<OrganizationDto> UpdateOrganization(UpdateOrganizationRequest request)
    {
        return await _organizationAppService.UpdateOrganizationAsync(request);
    }

    [HttpPost(Name = nameof(MoveOrganization))]
    public async Task<OrganizationDto> MoveOrganization(MoveOrganizationRequest request)
    {
        return await _organizationAppService.MoveOrganizationAsync(request);
    }

    [HttpPost(Name = nameof(DeleteOrganization))]
    public async Task DeleteOrganization(EntityDto<Guid> request)
    {
        await _organizationAppService.DeleteOrganizationAsync(request);
    }

    [HttpPost(Name = nameof(Up))]
    public async Task Up(EntityDto<Guid> request)
    {
        await _organizationAppService.UpAsync(request);
    }

    [HttpPost(Name = nameof(Down))]
    public async Task Down(EntityDto<Guid> request)
    {
        await _organizationAppService.DownAsync(request);
    }

    [HttpPost(Name = nameof(RemoveUserFromOrganization))]
    public async Task RemoveUserFromOrganization(UserToOrganizationRequest request)
    {
        await _organizationAppService.RemoveUserFromOrganizationAsync(request);
    }

    [HttpPost(Name = nameof(RemoveUsersFromOrganization))]
    public async Task RemoveUsersFromOrganization(UsersToOrganizationRequest request)
    {
        await _organizationAppService.RemoveUsersFromOrganizationAsync(request);
    }

    [HttpPost(Name = nameof(AddUsersToOrganization))]
    public async Task AddUsersToOrganization(UsersToOrganizationRequest request)
    {
        await _organizationAppService.AddUsersToOrganizationAsync(request);
    }

    [HttpPost(Name = nameof(FindUsers))]
    public async Task<PagedResponse<UsersDto>> FindUsers(FindOrganizationUsersRequest request)
    {
        return await _organizationAppService.FindUsersAsync(request);
    }

    [HttpPost(Name = nameof(ValidateOrganization))]
    [NoAudited]
    public async Task<ValidationResponse> ValidateOrganization(ValidationRequest<string, string> request)
    {
        return await _organizationAppService.ValidateOrganizationAsync(request);
    }
}