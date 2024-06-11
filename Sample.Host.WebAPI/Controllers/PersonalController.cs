using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Controllers;
using pandx.Wheel.Miscellaneous;
using pandx.Wheel.Models;
using Sample.Application.Authorization.Tokens;
using Sample.Application.Authorization.Tokens.Dto;
using Sample.Application.Menus;
using Sample.Application.Menus.Dto;
using Sample.Application.Personal;
using Sample.Application.Personal.Dto;

namespace Sample.Host.WebAPI.Controllers;
[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class PersonalController : WheelControllerBase
{
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly IMenuAppService _menuAppService;
    private readonly IPersonalAppService _personalAppService;
    private readonly ITokenAppService _tokenAppService;

    public PersonalController(IPersonalAppService personalAppService, ITokenAppService tokenAppService,
        IMenuAppService menuAppService, IClientInfoProvider clientInfoProvider)
    {
        _personalAppService = personalAppService;
        _tokenAppService = tokenAppService;
        _menuAppService = menuAppService;
        _clientInfoProvider = clientInfoProvider;
    }

    [HttpPost(Name = nameof(GetPersonal))]
    [NeedPermission]
    public async Task<PersonalResponse> GetPersonal()
    {
        return await _personalAppService.GetPersonalAsync();
    }

    [HttpPost(Name = nameof(GetToken))]
    [NoAudited]
    public async Task<TokenResponse> GetToken(GetTokenRequest request)
    {
        return await _tokenAppService.GetTokenAsync(request);
    }

    [HttpPost(Name = nameof(RefreshToken))]
    [NoAudited]
    public async Task<TokenResponse> RefreshToken(RefreshTokenRequest request)
    {
        return await _tokenAppService.RefreshTokenAsync(request, _clientInfoProvider.ClientIpAddress ?? string.Empty);
    }

    [HttpPost(Name = nameof(GetMenus))]
    [NeedPermission]
    public async Task<ListResponse<MenuDto>> GetMenus()
    {
        return await _menuAppService.GetMenusAsync();
    }
    [HttpPost(Name = nameof(UpdateUser))]
    [NeedPermission]
    public async Task<PersonalDto> UpdateUser(UpdatePersonalRequest request)
    {
        return await _personalAppService.UpdateUserAsync(request);
    }

    [HttpPost(Name = nameof(ChangePersonalPassword))]
    public async Task<bool> ChangePersonalPassword(ChangePersonalPasswordRequest request)
    {
        return await _personalAppService.ChangePasswordAsync(request);
    }

    [HttpPost(Name = nameof(GetLoginAttempts))]
    public  async Task<ListResponse<LoginAttemptDto>> GetLoginAttempts()
    {
        return await _personalAppService.GetLoginAttemptsAsync();
    }
    
    [HttpPost(Name=nameof(UploadAvatar))]
    public async Task UploadAvatar()
    {
        await _personalAppService.UploadAvatarAsync();
    }

    [HttpPost(Name = nameof(GetAvatar))]
    public async Task<string> GetAvatar()
    {
        return await _personalAppService.GetAvatarAsync();
    }

}