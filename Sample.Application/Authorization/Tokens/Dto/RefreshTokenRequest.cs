namespace Sample.Application.Authorization.Tokens.Dto;

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}