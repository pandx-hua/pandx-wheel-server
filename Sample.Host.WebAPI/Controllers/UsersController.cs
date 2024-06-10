using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Auditing;
using pandx.Wheel.Controllers;
using pandx.Wheel.Models;
using pandx.Wheel.Storage;
using Sample.Application.Authorization.Users;
using Sample.Application.Authorization.Users.Dto;

namespace Sample.Host.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class UsersController : WheelControllerBase
{
    private readonly IUserAppService _userAppService;

    public UsersController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    [HttpPost(Name = nameof(CreateOrUpdateUser))]
    public async Task CreateOrUpdateUser(CreateOrUpdateUserRequest request)
    {
        await _userAppService.CreateOrUpdateUserAsync(request);
    }

    [HttpPost(Name = nameof(GetPagedUsers))]
    public async Task<PagedResponse<UsersDto>> GetPagedUsers(GetUsersRequest request)
    {
        return await _userAppService.GetPagedUsersAsync(request);
    }

    [HttpPost(Name = nameof(ValidateUserName))]
    [NoAudited]
    public async Task<ValidationResponse> ValidateUserName(ValidationRequest<string, string> request)
    {
        return await _userAppService.ValidateUserNameAsync(request);
    }

    [HttpPost(Name = nameof(ValidateEmail))]
    [NoAudited]
    public async Task<ValidationResponse> ValidateEmail(ValidationRequest<string, string> request)
    {
        return await _userAppService.ValidateEmailAsync(request);
    }

    [HttpPost(Name = nameof(UnlockUser))]
    public async Task UnlockUser(EntityDto<Guid> request)
    {
        await _userAppService.UnlockUserAsync(request);
    }

    [HttpPost(Name = nameof(DeleteUser))]
    public async Task DeleteUser(EntityDto<Guid> request)
    {
        await _userAppService.DeleteUserAsync(request);
    }

    [HttpPost(Name = nameof(BatchDeleteUsers))]
    public async Task<int> BatchDeleteUsers(BatchDeleteUsersRequest request)
    {
        return await _userAppService.BatchDeleteUsersAsync(request);
    }

    [HttpPost(Name = nameof(ChangePassword))]
    public async Task<bool> ChangePassword(ChangePasswordRequest request)
    {
        return await _userAppService.ChangePasswordAsync(request);
    }

    [HttpPost(Name = nameof(ImportUsersFromExcel))]
    public async Task ImportUsersFromExcel()
    {
        await _userAppService.ImportUsersFromExcelAsync();
    }

    [HttpPost(Name = nameof(ExportUsersToExcel))]
    public async Task<CachedFile> ExportUsersToExcel(GetUsersRequest request)
    {
        return await _userAppService.ExportUsersToExcelAsync(request);
    }
    
    [HttpPost(Name = nameof(GetAvatarById))]
    public async Task<string> GetAvatarById(EntityDto<Guid> request)
    {
        return await _userAppService.GetAvatarByIdAsync(request);
    }
}