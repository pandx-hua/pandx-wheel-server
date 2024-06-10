using Microsoft.AspNetCore.Authorization;
using pandx.Wheel.Authorization.Users;

namespace pandx.Wheel.Authorization.Permissions;

public abstract class WheelPermissionAuthorizationHandler<TUser> : AuthorizationHandler<PermissionRequirement>
    where TUser : WheelUser
{
    private readonly IUserService<TUser> _userService;

    protected WheelPermissionAuthorizationHandler(IUserService<TUser> userService)
    {
        _userService = userService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User?.GetUserId() is { } userId &&
            await _userService.HasPermissionAsync(userId, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}