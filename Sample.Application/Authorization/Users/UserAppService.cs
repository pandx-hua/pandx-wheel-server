using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Authorization;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Events;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Extensions;
using pandx.Wheel.Models;
using pandx.Wheel.Notifications;
using pandx.Wheel.Organizations;
using pandx.Wheel.Storage;
using Sample.Application.Authorization.Users.Dto;
using Sample.Application.Authorization.Users.Exporting;
using Sample.Application.Authorization.Users.Importing;
using Sample.Domain.Authorization.Users;
using Sample.Domain.Notifications;

namespace Sample.Application.Authorization.Users;

public class UserAppService : SampleAppServiceBase, IUserAppService
{
    private readonly IBackgroundJobLauncher _backgroundJobLauncher;
    private readonly IBinaryObjectManager _binaryObjectManager;
    private readonly ICachedFileManager _cachedFileManager;
    private readonly IEventPublisher _eventPublisher;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUploadFileManager _uploadFileManager;
    private readonly IRepository<UserOrganization> _userOrganizationRepository;
    private readonly IUsersExcelExporter _usersExcelExporter;

    public UserAppService(IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IRepository<Organization, Guid> organizationRepository,
        INotificationSubscriptionManager notificationSubscriptionManager,
        INotificationPublisher notificationPublisher,
        IUploadFileManager uploadFileManager,
        ICachedFileManager cachedFileManager,
        IBackgroundJobLauncher backgroundJobLauncher,
        IUsersExcelExporter usersExcelExporter,
        IBinaryObjectManager binaryObjectManager,
        IRepository<UserOrganization> userOrganizationRepository)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
        _passwordHasher = passwordHasher;
        _organizationRepository = organizationRepository;
        _userOrganizationRepository = userOrganizationRepository;
        _notificationPublisher = notificationPublisher;
        _notificationSubscriptionManager = notificationSubscriptionManager;
        _uploadFileManager = uploadFileManager;
        _cachedFileManager = cachedFileManager;
        _backgroundJobLauncher = backgroundJobLauncher;
        _usersExcelExporter = usersExcelExporter;
        _binaryObjectManager = binaryObjectManager;
    }

    public async Task ImportUsersFromExcelAsync()
    {
        var cachedFile = await _uploadFileManager.UploadFileToCacheAsync();
        var backgroundJobData = new BackgroundJobData();
        backgroundJobData.Properties["CachedFile"] = cachedFile;
        backgroundJobData.Properties["CurrentUser"] = CurrentUser;
        await _backgroundJobLauncher.StartAsync<UsersExcelImportJob>(backgroundJobData);
    }

    public async Task<CachedFile> ExportUsersToExcelAsync(GetUsersRequest request)
    {
        var query = GetUsersFilteredQuery(request);
        var users = await query.OrderBy(request.Sorting!).ToListAsync();
        var dtos = users.Select(item =>
        {
            UsersDto dto = Mapper.Map<UsersDto>(item.User);
            dto.IsLockout = item.User.LockoutEnd != null && item.User.LockoutEnd > DateTimeOffset.Now;
            dto.IsWeixin = !string.IsNullOrWhiteSpace(item.User.OpenId);
            return dto;
        }).ToList();
        await FillRolesAsync(dtos);
        await FillOrganizationsAsync(dtos);

        return await _usersExcelExporter.ExportToExcelAsync(dtos);
    }

    public async Task<PagedResponse<UsersDto>> GetPagedUsersAsync(GetUsersRequest request)
    {
        var query = GetUsersFilteredQuery(request);
        var totalCount = await query.CountAsync();
        var users = await query.OrderBy(request.Sorting!).PageBy(request).ToListAsync();
        var dtos = users.Select(item =>
        {
            UsersDto dto = Mapper.Map<UsersDto>(item.User);
            dto.IsLockout = item.User.LockoutEnd != null && item.User.LockoutEnd > DateTimeOffset.Now;
            dto.IsWeixin = !string.IsNullOrWhiteSpace(item.User.OpenId);
            return dto;
        }).ToList();
        await FillRolesAsync(dtos);
        await FillOrganizationsAsync(dtos);
        return new PagedResponse<UsersDto>(totalCount, dtos);
    }

    public async Task<string> GetAvatarByIdAsync(EntityDto<Guid> request)
    {
        var user = await UserService.UserManager.FindByIdAsync(request.Id.ToString()) ??
                   throw new WheelException("没有发现指定的用户");
        if (user.AvatarId.HasValue)
        {
            if (await _binaryObjectManager.GetAsync(user.AvatarId.Value) is { } binaryObject)
            {
                return Convert.ToBase64String(binaryObject.Bytes);
            }
        }

        return string.Empty;
    }


    public async Task CreateOrUpdateUserAsync(CreateOrUpdateUserRequest request)
    {
        if (request.User.Id is not null)
        {
            await UpdateUserAsync(request);
        }
        else
        {
            await CreateUserAsync(request);
        }
    }

    public async Task DeleteUserAsync(EntityDto<Guid> request)
    {
        if (request.Id == CurrentUser.GetUserId())
        {
            throw new WheelException("自己不能删除自己");
        }

        var user = await UserService.UserManager.FindByIdAsync(request.Id.ToString());
        if (user is null)
        {
            throw new WheelException("没有发现用户");
        }

        CheckErrors(await UserService.UserManager.DeleteAsync(user));
        //update organizations
        await UserService.RemoveFromOrganizationsAsync(user);
        await _eventPublisher.PublishAsync(new UserDeletedEvent<ApplicationUser>(user));
    }

    public async Task<int> BatchDeleteUsersAsync(BatchDeleteUsersRequest request)
    {
        var i = 0;
        foreach (var userId in request.UserIds)
        {
            try
            {
                await DeleteUserAsync(new EntityDto<Guid> { Id = userId });
            }
            catch
            {
                continue;
            }

            i++;
        }

        return i;
    }

    public async Task UnlockUserAsync(EntityDto<Guid> request)
    {
        if (request.Id == CurrentUser.GetUserId())
        {
            throw new WheelException("自己不能解锁自己");
        }

        var user = await UserService.UserManager.FindByIdAsync(request.Id.ToString());
        if (user is null)
        {
            throw new WheelException("没有发现用户");
        }

        user.Unlock();
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var currentUserId = CurrentUser.GetUserId();
        var currentUser = (await UserService.UserManager.FindByIdAsync(currentUserId.ToString()!))!;
        var signInResult =
            await UserService.SignInManager.PasswordSignInAsync(currentUser, request.SuperPassword, false, false);
        if (!signInResult.Succeeded)
        {
            throw new WheelException($"{currentUser.Name} 的密码无法通过验证");
        }

        //TODO
        var roles = await UserService.UserManager.GetRolesAsync(currentUser);
        if (!roles.Contains(RoleNames.Admin))
        {
            throw new WheelException("只有具有超级权限的用户才可以修改用户密码");
        }

        var user = await UserService.UserManager.FindByIdAsync(request.UserId.ToString());
        if (user is not null)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        }

        return true;
    }

    public async Task<ValidationResponse> ValidateUserNameAsync(ValidationRequest<string, string> request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            var one = await UserService.UserManager.Users.IgnoreQueryFilters()
                .SingleOrDefaultAsync(u => u.UserName == request.Value);
            if (one != null)
            {
                return new ValidationResponse
                {
                    Status = false,
                    Message = "账号 " + request.Value + " 已被占用"
                };
            }

            return new ValidationResponse
            {
                Status = true
            };
        }
        else
        {
            var one = await UserService.UserManager.Users.IgnoreQueryFilters().SingleOrDefaultAsync(u =>
                u.UserName == request.Value && u.Id.ToString() != request.Id);
            if (one != null)
            {
                return new ValidationResponse
                {
                    Status = false,
                    Message = "账号 " + request.Value + " 已被占用"
                };
            }

            return new ValidationResponse
            {
                Status = true
            };
        }
    }

    public async Task<ValidationResponse> ValidateEmailAsync(ValidationRequest<string, string> request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            var one = await UserService.UserManager.Users.IgnoreQueryFilters()
                .SingleOrDefaultAsync(u => u.Email == request.Value);
            if (one != null)
            {
                return new ValidationResponse
                {
                    Status = false,
                    Message = "Email地址 " + request.Value + " 已被占用"
                };
            }

            return new ValidationResponse
            {
                Status = true
            };
        }
        else
        {
            var one = await UserService.UserManager.Users.IgnoreQueryFilters().SingleOrDefaultAsync(u =>
                u.Email == request.Value && u.Id.ToString() != request.Id);
            if (one != null)
            {
                return new ValidationResponse
                {
                    Status = false,
                    Message = "Email地址 " + request.Value + " 已被占用"
                };
            }

            return new ValidationResponse
            {
                Status = true
            };
        }
    }

    private async Task CreateUserAsync(CreateOrUpdateUserRequest request)
    {
        var user = Mapper.Map<ApplicationUser>(request.User);

        CheckErrors(await UserService.UserManager.CreateAsync(user, request.User.Password!));
        await _unitOfWork.CommitAsync();

        var staticRoleNames = (await RoleService.GetStaticRolesAsync()).Select(r => r.Name!).ToList();
        var assignedRoleNames = request.AssignedRoleNames.Union(staticRoleNames);
        //update role
        foreach (var assignedRoleName in assignedRoleNames)
        {
            CheckErrors(await UserService.UserManager.AddToRoleAsync(user, assignedRoleName));
        }

        //update organizations
        foreach (var assignedOrganizationId in request.AssignedOrganizationIds)
        {
            await UserService.AddToOrganizationAsync(user.Id, assignedOrganizationId);
        }

        await _notificationSubscriptionManager.SubscribeToAllNotificationsAsync(new UserIdentifier(user.Id.ToString()));
        await _notificationPublisher.PublishAsync(SampleNotificationNames.Test, userIds: new[] { user.Id });
        await _eventPublisher.PublishAsync(new UserCreatedEvent<ApplicationUser>(user));
    }

    private async Task UpdateUserAsync(CreateOrUpdateUserRequest request)
    {
        var user = await UserService.UserManager.FindByIdAsync(request.User.Id.ToString()!) ??
                   throw new Exception("没有发现指定的用户");
        Mapper.Map(request.User, user);
        CheckErrors(await UserService.UserManager.UpdateAsync(user));

        if (!request.User.Password.IsNullOrEmpty())
        {
            var token = await UserService.UserManager.GeneratePasswordResetTokenAsync(user);
            CheckErrors(await UserService.UserManager.ResetPasswordAsync(user, token, request.User.Password!));
        }

        //update role
        CheckErrors(await UserService.SetRolesAsync(user, request.AssignedRoleNames));
        //update organizations
        await UserService.SetOrganizationsAsync(user, request.AssignedOrganizationIds.ToArray());
        await _eventPublisher.PublishAsync(new UserUpdatedEvent<ApplicationUser>(user));
    }

    private void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors();
    }

    private IQueryable<dynamic> GetUsersFilteredQuery(GetUsersRequest request)
    {
        var query = from user in UserService.UserManager.Users
            select new
            {
                User = user
            };
        query = query
            .Where(x => x.User.CreationTime >= request.StartTime && x.User.CreationTime <= request.EndTime)
            .WhereIf(request.Gender.Count > 0, x => request.Gender.Contains(x.User.Gender))
            .WhereIf(request.IsActive.Count > 0, x => request.IsActive.Contains(x.User.IsActive))
            .WhereIf(request.IsWeixin.Count == 1 && request.IsWeixin.Contains(true),
                x => !string.IsNullOrWhiteSpace(x.User.OpenId))
            .WhereIf(request.IsWeixin.Count == 1 && request.IsWeixin.Contains(false),
                x => string.IsNullOrWhiteSpace(x.User.OpenId))
            .WhereIf(request.IsLockout.Count == 1 && request.IsLockout.Contains(true),
                x => x.User.LockoutEnd != null && x.User.LockoutEnd > DateTimeOffset.Now)
            .WhereIf(request.IsLockout.Count == 1 && request.IsLockout.Contains(false),
                x => x.User.LockoutEnd == null || x.User.LockoutEnd <= DateTimeOffset.Now)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Filter),
                x => x.User.UserName!.Contains(request.Filter!) ||
                     x.User.Name.Contains(request.Filter!) ||
                     x.User.Email!.Contains(request.Filter!) ||
                     x.User.PhoneNumber!.Contains(request.Filter!));
        return query;
    }

    private async Task FillOrganizationsAsync(IReadOnlyCollection<UsersDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var query = from userOrganization in _userOrganizationRepository.GetAll()
                    .Where(uo => uo.UserId == dto.Id)
                join o in _organizationRepository.GetAll()
                    on userOrganization.OrganizationId equals o.Id
                select o;
            dto.Organizations = Mapper.Map<List<UserOrganizationsDto>>(await query.ToListAsync());
        }
    }

    private async Task FillRolesAsync(IReadOnlyCollection<UsersDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var userRoleDtos = new List<UserRolesDto>();
            foreach (var roleName in await UserService.UserManager.GetRolesAsync(
                         await UserService.UserManager.FindByIdAsync(dto.Id.ToString()) ??
                         throw new InvalidOperationException("没有发现用户")))
            {
                userRoleDtos.Add(Mapper.Map<UserRolesDto>(await RoleService.RoleManager.FindByNameAsync(roleName)));
            }

            dto.Roles = userRoleDtos;
        }
    }
}