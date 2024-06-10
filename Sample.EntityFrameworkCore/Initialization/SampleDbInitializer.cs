using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pandx.Wheel.Authorization;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Authorization.Roles;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Initializers;
using pandx.Wheel.Notifications;
using Sample.Domain;
using Sample.Domain.Authorization.Roles;
using Sample.Domain.Authorization.Users;

namespace Sample.EntityFrameworkCore.Initialization;

public class SampleDbInitializer : IDbInitializer
{
    private readonly ILogger<SampleDbInitializer> _logger;
    private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IPermissionManager _permissionManager;
    private readonly IRoleService<ApplicationRole> _roleService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService<ApplicationUser> _userService;

    public SampleDbInitializer(IUserService<ApplicationUser> userService, IRoleService<ApplicationRole> roleService,
        ILogger<SampleDbInitializer> logger, IUnitOfWork unitOfWork,
        IPasswordHasher<ApplicationUser> passwordHasher, IServiceProvider serviceProvider,
        IPermissionManager permissionManager,
        INotificationSubscriptionManager notificationSubscriptionManager)
    {
        _userService = userService;
        _roleService = roleService;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _serviceProvider = serviceProvider;
        _permissionManager = permissionManager;
        _notificationSubscriptionManager = notificationSubscriptionManager;
    }

    public async Task InitializeAsync()
    {
        await RoleAndUserCreator();
        await CustomCreator();
    }

    private async Task RoleAndUserCreator()
    {
        var role = await _roleService.RoleManager.FindByNameAsync(SampleConsts.Role.Name);
        if (role is null)
        {
            role = new ApplicationRole
            {
                Name = SampleConsts.Role.Name,
                DisplayName = SampleConsts.Role.Name,
                IsStatic = SampleConsts.Role.IsStatic
            };
            await _roleService.RoleManager.CreateAsync(role);
            await _unitOfWork.CommitAsync();
        }

        var permissions = _permissionManager.GetExpandedPermissions();
        foreach (var permission in permissions)
        {
            var claims = await _roleService.RoleManager.GetClaimsAsync(role);
            var claim = new Claim(WheelClaimTypes.Permission,
                $"{WheelClaimTypes.Permission}.{permission.Resource}.{permission.Action}");
            if (claims.All(c => c.Value != claim.Value))
            {
                await _roleService.RoleManager.AddClaimAsync(role, claim);
            }
        }

        var user = await _userService.UserManager.FindByNameAsync(SampleConsts.User.UserName);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = SampleConsts.User.UserName,
                Name = SampleConsts.User.UserName,
                Email = SampleConsts.User.Email,
                EmailConfirmed = true,
                IsActive = true
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, SampleConsts.User.Password);
            await _userService.UserManager.CreateAsync(user);
            await _userService.UserManager.AddToRoleAsync(user, role.Name!);
            await _unitOfWork.CommitAsync();
        }

        await _notificationSubscriptionManager.SubscribeToAllNotificationsAsync(new UserIdentifier(user.Id.ToString()));
    }

    private async Task CustomCreator()
    {
        var creators = _serviceProvider.GetRequiredService<IEnumerable<ICustomCreator>>().ToArray();
        foreach (var creator in creators)
        {
            await creator.InitializeAsync();
        }
    }
}