using pandx.Wheel.Application.Services;
using pandx.Wheel.Authorization.Roles;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Extensions;
using Sample.Domain.Authorization.Roles;
using Sample.Domain.Authorization.Users;

namespace Sample.Application;

public abstract class SampleAppServiceBase : ApplicationServiceBase
{
    [Injection] public IUserService<ApplicationUser> UserService { get; set; } = default!;
    [Injection] public IRoleService<ApplicationRole> RoleService { get; set; } = default!;

    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var user = await UserService.UserManager.FindByIdAsync(CurrentUser.GetUserId().ToJsonString());
        _ = user ?? throw new Exception("没有找到指定的用户");
        return user;
    }

    public async Task<IList<string>> GetCurrentRolesAsync()
    {
        var user = await UserService.UserManager.FindByIdAsync(CurrentUser.GetUserId().ToJsonString());
        _ = user ?? throw new Exception("没有找到指定的用户");
        return await UserService.UserManager.GetRolesAsync(user);
    }
}