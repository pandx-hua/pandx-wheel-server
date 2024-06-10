using pandx.Wheel.Authorization.Users;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Authorization.Tokens;

public interface ITokenService<in TUser> : ITransientDependency where TUser : WheelUser
{
    string CreateAccessToken(TUser user, string ipAddress);
    string CreateRefreshToken();
    string CreateEncryptedAccessToken(string accessToken);
}