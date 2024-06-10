using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Authorization;
using pandx.Wheel.Authorization.Roles;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Notifications;
using pandx.Wheel.Organizations;
using pandx.Wheel.Storage;
using Quartz;
using Quartz.Util;
using Sample.Application.Authorization.Users.Importing.Dto;
using Sample.Domain.Authorization.Roles;
using Sample.Domain.Authorization.Users;
using Sample.Domain.Notifications;
using Sample.EntityFrameworkCore;

namespace Sample.Application.Authorization.Users.Importing;

public class UsersExcelImportJob : BackgroundJob
{
    private readonly ICachedFileManager _cachedFileManager;
    private readonly SampleDbContext _dbContext;
    private readonly IInvalidUsersExcelExporter _invalidUsersExcelExporter;
    private readonly IMapper _mapper;
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IEnumerable<IPasswordValidator<ApplicationUser>> _passwordValidators;
    private readonly IRoleService<ApplicationRole> _roleService;
    private readonly ISampleNotifier _sampleNotifier;
    private readonly IUserService<ApplicationUser> _userService;
    private readonly IUsersExcelImporter _usersExcelImporter;

    public UsersExcelImportJob(
        IUsersExcelImporter usersExcelImporter,
        ICachedFileManager cachedFileManager,
        ISampleNotifier sampleNotifier,
        IMapper mapper,
        IUserService<ApplicationUser> userService,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IRepository<Organization, Guid> organizationRepository,
        IRoleService<ApplicationRole> roleService,
        IInvalidUsersExcelExporter invalidUsersExcelExporter,
        SampleDbContext dbContext,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators)
    {
        _usersExcelImporter = usersExcelImporter;
        _cachedFileManager = cachedFileManager;
        _sampleNotifier = sampleNotifier;
        _mapper = mapper;
        _passwordValidators = passwordValidators;
        _userService = userService;
        _passwordHasher = passwordHasher;
        _organizationRepository = organizationRepository;
        _roleService = roleService;
        _invalidUsersExcelExporter = invalidUsersExcelExporter;
        _dbContext = dbContext;
    }

    public override BackgroundJobData BackgroundJobData { get; set; } = default!;

    public override async Task Execute(IJobExecutionContext context)
    {
        var cachedFile = BackgroundJobData["CachedFile"] as CachedFile;
        var currentUser = BackgroundJobData["CurrentUser"] as CurrentUser;
        var bytes = await _cachedFileManager.GetFileAsync(cachedFile!.Token, cachedFile.Name);
        var users = _usersExcelImporter.GetUsersFromExcel(bytes!);
        if (!users.Any())
        {
            var notificationData = new NotificationData();
            notificationData["Message"] = "选择的Excel文件无法通过验证，请检查";
            await _sampleNotifier.SendInvalidExcelNotification(notificationData, new[] { currentUser!.GetUserId() });
            return;
        }

        await CreateUsers(users, currentUser!);
    }

    private async Task CreateUsers(List<ImportedUserDto> users, CurrentUser currentUser)
    {
        var invalidUsers = new List<ImportedUserDto>();
        foreach (var user in users)
        {
            if (user.CanBeImported())
            {
                try
                {
                    await CreateUser(user);
                }
                catch (Exception exception)
                {
                    user.Exception = exception.Message;
                    invalidUsers.Add(user);
                }
            }
            else
            {
                invalidUsers.Add(user);
            }
        }

        await ProcessImportedUsersResultAsync(invalidUsers, currentUser);
    }

    private async Task CreateUser(ImportedUserDto dto)
    {
        var user = _mapper.Map<ApplicationUser>(dto);
        if (!dto.GenderName.IsNullOrWhiteSpace())
        {
            user.Gender = dto.GenderName == "男" ? Gender.Male : Gender.Female;
        }
        else
        {
            throw new WheelException("性别不能为空");
        }

        if (!dto.GenderName.IsNullOrWhiteSpace())
        {
            user.IsActive = dto.ActiveName == "是";
        }
        else
        {
            throw new WheelException("激活状态不能为空");
        }

        if (!dto.Password.IsNullOrWhiteSpace())
        {
            foreach (var validator in _passwordValidators)
            {
                var identityResult = await validator.ValidateAsync(_userService.UserManager, user, dto.Password);
                if (!identityResult.Succeeded)
                {
                    throw new WheelException(identityResult.Errors.First().Description);
                }
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        }
        else
        {
            throw new WheelException("密码不能为空");
        }


        var result = await _userService.UserManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            throw new WheelException(result.Errors.First().Description);
        }

        if (dto.AssignedOrganizations.Any())
        {
            foreach (var code in dto.AssignedOrganizations)
            {
                var organization =
                    await _organizationRepository.FirstOrDefaultAsync(o => o.Code == code);
                if (organization is not null)
                {
                    await _userService.AddToOrganizationAsync(user, organization);
                }
                else
                {
                    // 删除用户, 因为用户已经创建成功，但是部门不存在，采用原始SQL语句，跳过软删除
                    await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM USERS WHERE ID={0}", user.Id);
                    throw new WheelException($"代码为 {code} 的部门不存在");
                }
            }
        }

        if (dto.AssignedRoles.Any())
        {
            foreach (var name in dto.AssignedRoles)
            {
                try
                {
                    var roleName = GetRoleNameFromDisplayName(name, _roleService.RoleManager.Roles.ToList());
                    await _userService.UserManager.AddToRoleAsync(user, roleName);
                }
                catch (Exception exception)
                {
                    // 删除用户, 因为用户已经创建成功，但是角色不存在，采用原始SQL语句，跳过软删除
                    await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM USERS WHERE ID={0}", user.Id);
                }
            }
        }
    }

    private async Task ProcessImportedUsersResultAsync(List<ImportedUserDto> invalidUsers, CurrentUser currentUser)
    {
        if (invalidUsers.Any())
        {
            var cachedFile = await _invalidUsersExcelExporter.ExportToExcelAsync(invalidUsers);
            await _cachedFileManager.SaveFileAsync(cachedFile.Token, cachedFile.Name);

            var notificationData = new NotificationData();
            notificationData["Message"] = "部分用户导入失败";
            notificationData["DownloadToken"] = cachedFile.Token + "-" + cachedFile.Name;
            await _sampleNotifier.SendInvalidUsersNotification(notificationData, new[] { currentUser.GetUserId() });
        }
        else
        {
            var notificationData = new NotificationData();
            notificationData["Message"] = "用户全部导入成功";
            await _sampleNotifier.SendCommonMessageNotification(notificationData, NotificationSeverity.Success,
                new[] { currentUser.GetUserId() });
        }
    }

    private string GetRoleNameFromDisplayName(string displayName, List<ApplicationRole> roles)
    {
        var role = roles.FirstOrDefault(r => r.DisplayName!.ToLowerInvariant() == displayName.ToLowerInvariant());
        if (role is not null)
        {
            return role.Name!;
        }

        throw new WheelException($"角色 {displayName} 不存在");
    }
}