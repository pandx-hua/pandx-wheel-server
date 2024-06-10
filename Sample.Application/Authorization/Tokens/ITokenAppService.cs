using pandx.Wheel.Application.Services;
using Sample.Application.Authorization.Tokens.Dto;

namespace Sample.Application.Authorization.Tokens;

public interface ITokenAppService : IApplicationService
{
    Task<TokenResponse> GetTokenAsync(GetTokenRequest request);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
}