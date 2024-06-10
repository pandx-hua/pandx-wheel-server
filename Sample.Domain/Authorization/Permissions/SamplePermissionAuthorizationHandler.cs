using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Authorization.Users;
using Sample.Domain.Authorization.Users;

namespace Sample.Domain.Authorization.Permissions;

public class SamplePermissionAuthorizationHandler : WheelPermissionAuthorizationHandler<ApplicationUser>
{
    public SamplePermissionAuthorizationHandler(IUserService<ApplicationUser> userService) : base(userService)
    {
    }
}