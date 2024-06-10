using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Security;

namespace pandx.Wheel.Authorization.Tokens;

public abstract class WheelTokenService<TUser> : ITokenService<TUser> where TUser : WheelUser
{
    private readonly IConfiguration _configuration;
    private readonly SecuritySettings _securitySettings;

    protected WheelTokenService(IConfiguration configuration,
        IOptions<SecuritySettings> options)
    {
        _configuration = configuration;
        _securitySettings = options.Value;
    }

    public string CreateAccessToken(TUser user, string ipAddress)
    {
        return CreateToken(CreateJwtClaims(user, ipAddress),
            TimeSpan.FromMinutes(_securitySettings.JwtBearer.AccessTokenExpirationInMinutes));
    }

    public string CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var randomGenerator = RandomNumberGenerator.Create();
        randomGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string CreateEncryptedAccessToken(string accessToken)
    {
        return SimpleStringCipher.Instance.Encrypt(accessToken);
    }

    private IEnumerable<Claim> CreateJwtClaims(TUser user, string ipAddress)
    {
        var claims = new List<Claim>();

        claims.AddRange(new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
            new Claim(WheelClaimTypes.UserIdentifier, user.Id.ToString()),
            new Claim(WheelClaimTypes.UserName, user.UserName!),
            new Claim(WheelClaimTypes.TrueName, user.Name),
            new Claim(WheelClaimTypes.Email, user.Email!),
            new Claim(WheelClaimTypes.IpAddress, ipAddress)
        });
        return claims;
    }

    private string CreateToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
    {
        var now = DateTime.UtcNow;
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettings.JwtBearer.SecurityKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _securitySettings.JwtBearer.Issuer,
            _securitySettings.JwtBearer.Audience,
            claims,
            now,
            expiration is null ? null : now.Add(expiration.Value),
            signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}