using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Controllers;
using pandx.Wheel.Models;
using pandx.Wheel.Storage;
using Sample.Application.Authorization.Users;
using Sample.Application.Authorization.Users.Dto;
using Sample.Domain.Authorization.Permissions;

namespace Sample.Host.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : WheelControllerBase
{
    private readonly IUserAppService _userAppService;

    public UsersController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Create)]
    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Update)]
    [HttpPost(Name = nameof(CreateOrUpdateUser))]
    public async Task CreateOrUpdateUser(CreateOrUpdateUserRequest request)
    {
        await _userAppService.CreateOrUpdateUserAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Search)]
    [HttpPost(Name = nameof(GetPagedUsers))]
    public async Task<PagedResponse<UsersDto>> GetPagedUsers(GetUsersRequest request)
    {
        return await _userAppService.GetPagedUsersAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Create)]
    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Update)]
    [HttpPost(Name = nameof(ValidateUserName))]
    [NoAudited]
    public async Task<ValidationResponse> ValidateUserName(ValidationRequest<string, string> request)
    {
        return await _userAppService.ValidateUserNameAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Create)]
    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Update)]
    [HttpPost(Name = nameof(ValidateEmail))]
    [NoAudited]
    public async Task<ValidationResponse> ValidateEmail(ValidationRequest<string, string> request)
    {
        return await _userAppService.ValidateEmailAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Unlock)]
    [HttpPost(Name = nameof(UnlockUser))]
    public async Task UnlockUser(EntityDto<Guid> request)
    {
        await _userAppService.UnlockUserAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Delete)]
    [HttpPost(Name = nameof(DeleteUser))]
    public async Task DeleteUser(EntityDto<Guid> request)
    {
        await _userAppService.DeleteUserAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Delete)]
    [HttpPost(Name = nameof(BatchDeleteUsers))]
    public async Task<int> BatchDeleteUsers(BatchDeleteUsersRequest request)
    {
        return await _userAppService.BatchDeleteUsersAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Change)]
    [HttpPost(Name = nameof(ChangePassword))]
    public async Task<bool> ChangePassword(ChangePasswordRequest request)
    {
        return await _userAppService.ChangePasswordAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Import)]
    [HttpPost(Name = nameof(ImportUsersFromExcel))]
    public async Task ImportUsersFromExcel()
    {
        await _userAppService.ImportUsersFromExcelAsync();
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Export)]
    [HttpPost(Name = nameof(ExportUsersToExcel))]
    public async Task<CachedFile> ExportUsersToExcel(GetUsersRequest request)
    {
        return await _userAppService.ExportUsersToExcelAsync(request);
    }

    [NeedPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Browse)]
    [HttpPost(Name = nameof(GetAvatarById))]
    public async Task<string> GetAvatarById(EntityDto<Guid> request)
    {
        return await _userAppService.GetAvatarByIdAsync(request);
    }
}