namespace Sample.Application.Authorization.Tokens.Dto;

public class TokenResponse
{
    public string AccessToken { get; set; } = default!;
    public DateTime AccessTokenExpirationUtcTime { get; set; }
    public string EncryptedToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpirationUtcTime { get; set; }
}