using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using pandx.Wheel.Authorization.Tokens;
using pandx.Wheel.Security;
using Sample.Domain.Authorization.Users;

namespace Sample.Domain.Authorization.Tokens;

public class TokenService : WheelTokenService<ApplicationUser>
{
    public TokenService(IConfiguration configuration, IOptions<SecuritySettings> options) : base(configuration, options)
    {
    }
}