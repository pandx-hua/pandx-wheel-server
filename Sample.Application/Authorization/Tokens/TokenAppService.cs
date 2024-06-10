using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization;
using pandx.Wheel.Authorization.Logins;
using pandx.Wheel.Authorization.Tokens;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Miscellaneous;
using pandx.Wheel.Security;
using Sample.Application.Authorization.Tokens.Dto;
using Sample.Domain.Authorization.Users;

namespace Sample.Application.Authorization.Tokens;

public class TokenAppService : SampleAppServiceBase, ITokenAppService
{
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly IOptions<SecuritySettings> _securitySettingsOptions;
    private readonly ITokenService<ApplicationUser> _tokenService;
    private readonly ILoginService _loginService;

    public TokenAppService(ITokenService<ApplicationUser> tokenService,
        IOptions<SecuritySettings> securitySettingsOptions,
        IClientInfoProvider clientInfoProvider,
        ILoginService loginService)
    {
        _tokenService = tokenService;
        _securitySettingsOptions = securitySettingsOptions;
        _clientInfoProvider = clientInfoProvider;
        _loginService = loginService;
    }

    public async Task<TokenResponse> GetTokenAsync(GetTokenRequest request)
    {
        var loginAttempt = new LoginAttempt();

        var user = await UserService.UserManager.FindByEmailAsync(request.UserNameOrEmail) ??
                   await UserService.UserManager.FindByNameAsync(request.UserNameOrEmail);

        if (user is null)
        {
            await _loginService.CreateLoginAttemptAsync(request.UserNameOrEmail,
                LoginResultType.InvalidUserNameOrEmail);
            throw new WheelException("输入的账号或密码错误，请重试");
        }

        var result = await UserService.SignInManager.PasswordSignInAsync(user, request.Password, request.IsPersistent,
            true);
        if (result.IsLockedOut)
        {
            await _loginService.CreateLoginAttemptAsync(request.UserNameOrEmail, LoginResultType.UserIsLockedOut,
                user.Id);
            throw new WheelException("输入的账号已被锁定，请联系管理员");
        }

        if (!result.Succeeded)
        {
            await _loginService.CreateLoginAttemptAsync(request.UserNameOrEmail, LoginResultType.InvalidPassword,
                user.Id);
            throw new WheelException("输入的账号或密码错误，请重试");
        }

        if (!user.IsActive)
        {
            await _loginService.CreateLoginAttemptAsync(request.UserNameOrEmail, LoginResultType.UserIsNotActive,
                user.Id);
            throw new WheelException("输入的账号未激活，请联系管理员");
        }

        var ipAddress = _clientInfoProvider.ClientIpAddress ?? string.Empty;
        
        await _loginService.CreateLoginAttemptAsync(request.UserNameOrEmail, LoginResultType.Success,
            user.Id);
        return await CreateTokenAndUpdateUser(user, ipAddress);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        var principal = GetPrincipalFromExpiredAccessToken(request.AccessToken);
        var userName = principal.GetUserName();
        var user = await UserService.UserManager.FindByNameAsync(userName!);
        _ = user ?? throw new WheelException("RefreshToken无法通过验证");

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpirationUtcTime <= DateTime.UtcNow)
        {
            throw new WheelException("RefreshToken无法通过验证");
        }

        return await CreateTokenAndUpdateUser(user, ipAddress);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettingsOptions.Value.JwtBearer.SecurityKey)),
            ValidateIssuer = false,
            ValidIssuer = _securitySettingsOptions.Value.JwtBearer.Issuer,

            ValidateAudience = false,
            ValidAudience = _securitySettingsOptions.Value.JwtBearer.Audience,

            ValidateLifetime = false,

            ClockSkew = TimeSpan.Zero
        };
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var principal =
            jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
        {
            throw new WheelException("AccessToken无法通过验证");
        }

        return principal;
    }

    private async Task<TokenResponse> CreateTokenAndUpdateUser(ApplicationUser user, string ipAddress)
    {
        var accessToken = _tokenService.CreateAccessToken(user, ipAddress);
        var accessTokenExpirationUtcTime =
            DateTime.UtcNow.AddMinutes(_securitySettingsOptions.Value.JwtBearer.AccessTokenExpirationInMinutes);
        var encryptedToken = _tokenService.CreateEncryptedAccessToken(accessToken);
        user.RefreshToken = _tokenService.CreateRefreshToken();
        user.RefreshTokenExpirationUtcTime =
            DateTime.UtcNow.AddDays(_securitySettingsOptions.Value.JwtBearer.RefreshTokenExpirationInDays);
        await UserService.UserManager.UpdateAsync(user);

        return new TokenResponse
        {
            AccessToken = accessToken,
            AccessTokenExpirationUtcTime = accessTokenExpirationUtcTime,
            EncryptedToken = encryptedToken,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpirationUtcTime = user.RefreshTokenExpirationUtcTime.Value
        };
    }
}