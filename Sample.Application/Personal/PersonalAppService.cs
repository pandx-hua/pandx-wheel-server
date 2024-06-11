using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Authorization;
using pandx.Wheel.Authorization.Logins;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Helpers;
using pandx.Wheel.Models;
using pandx.Wheel.Storage;
using Sample.Application.Personal.Dto;
using Sample.Domain.Authorization.Users;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace Sample.Application.Personal;

public class PersonalAppService : SampleAppServiceBase, IPersonalAppService
{
    private const long MaxAvatarBytes = 1024 * 1024 * 1;
    private readonly IBinaryObjectManager _binaryObjectManager;
    private readonly ICachedFileManager _cachedFileManager;
    private readonly IRepository<LoginAttempt, Guid> _loginAttemptRepository;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IPermissionManager _permissionManager;
    private readonly IUploadFileManager _uploadFileManager;

    public PersonalAppService(
        IPermissionManager permissionManager,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IUploadFileManager uploadFileManager,
        ICachedFileManager cachedFileManager,
        IBinaryObjectManager binaryObjectManager,
        IRepository<LoginAttempt, Guid> loginAttemptRepository)
    {
        _permissionManager = permissionManager;
        _passwordHasher = passwordHasher;
        _loginAttemptRepository = loginAttemptRepository;
        _uploadFileManager = uploadFileManager;
        _cachedFileManager = cachedFileManager;
        _binaryObjectManager = binaryObjectManager;
    }

    public async Task<PersonalResponse> GetPersonalAsync()
    {
        return new PersonalResponse
        {
            GrantedPermissions = await GetGrantedPermissions(),
            User = await GetUser()
        };
    }

    public async Task<string> GetAvatarAsync()
    {
        var userId = CurrentUser.GetUserId();
        var user = await UserService.UserManager.FindByIdAsync(userId.ToString()) ??
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

    public async Task UploadAvatarAsync()
    {
        var cachedFile = await _uploadFileManager.UploadFileToCacheAsync();
        var bytes = await _cachedFileManager.GetFileAsync(cachedFile.Token, cachedFile.Name);
        if (bytes is null)
        {
            throw new WheelException("没有发现指定的文件");
        }

        IImageFormat[] imageFormats = [JpegFormat.Instance, PngFormat.Instance, GifFormat.Instance];

        if (!imageFormats.Contains(ImageFormatHelper.GetRawImageFormat(bytes)))
        {
            throw new WheelException("图像格式不正确，仅支持JPG、PNG、GIF格式");
        }

        if (bytes.Length > MaxAvatarBytes)
        {
            throw new WheelException($"头像文件大小超出 {MaxAvatarBytes / (1024 * 1024)} MB的限制");
        }

        var userId = CurrentUser.GetUserId();
        var user = await UserService.UserManager.FindByIdAsync(userId.ToString()) ??
                   throw new WheelException("没有发现指定的用户");
        if (user.AvatarId.HasValue)
        {
            await _binaryObjectManager.DeleteAsync(user.AvatarId.Value);
        }

        //此段代码仅在windows上被支持，下次迭代时需要移除
        // using var bitmap = new Bitmap(new MemoryStream(bytes));
        // var newBitmap = bitmap.Width >= bitmap.Height
        //     ? new Bitmap(bitmap, bitmap.Width * 150 / bitmap.Height, 150)
        //     : new Bitmap(bitmap, 150, bitmap.Height * 150 / bitmap.Width);
        //
        // var bitmapCrop = newBitmap.Clone(new Rectangle(0, 0, 150, 150), newBitmap.PixelFormat);
        // await using var stream = new MemoryStream();
        // bitmapCrop.Save(stream, bitmap.RawFormat);
        // var finalBytes = stream.ToArray();

        var binaryObject = new BinaryObject
        {
            Bytes = bytes,
            Description = $"用户{user.UserName}/{user.Name} 的头像",
            Length = bytes.Length
        };
        await _binaryObjectManager.SaveAsync(binaryObject);
        user.AvatarId = binaryObject.Id;
    }


    public async Task<ListResponse<LoginAttemptDto>> GetLoginAttemptsAsync()
    {
        var loginAttempts = await (await _loginAttemptRepository.GetAllAsync())
            .Where(l => l.UserId == CurrentUser.GetUserId()).OrderByDescending(l => l.CreationTime)
            .Take(20).ToListAsync();
        return new ListResponse<LoginAttemptDto>(Mapper.Map<List<LoginAttemptDto>>(loginAttempts));
    }

    public async Task<bool> ChangePasswordAsync(ChangePersonalPasswordRequest request)
    {
        var userId = CurrentUser.GetUserId();
        var user = await UserService.UserManager.FindByIdAsync(userId.ToString()) ??
                   throw new WheelException("没有发现指定的用户");
        var signInResult =
            await UserService.SignInManager.PasswordSignInAsync(user, request.CurrentPassword, false, false);
        if (!signInResult.Succeeded)
        {
            throw new WheelException($"{user.Name} 的密码无法通过验证");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        return true;
    }

    public async Task<PersonalDto> UpdateUserAsync(UpdatePersonalRequest request)
    {
        var userId = CurrentUser.GetUserId();
        var user = await UserService.UserManager.FindByIdAsync(userId.ToString()) ??
                   throw new Exception("没有发现指定的用户");
        user.IsActive = true;
        Mapper.Map(request.User, user);
        CheckErrors(await UserService.UserManager.UpdateAsync(user));

        return Mapper.Map<PersonalDto>(user);
    }

    private async Task<PersonalDto> GetUser()
    {
        if (CurrentUser.IsAuthenticated())
        {
            var user = await UserService.UserManager.FindByIdAsync(CurrentUser.GetUserId().ToString());
            return Mapper.Map<PersonalDto>(user);
        }

        return new PersonalDto();
    }

    private async Task<Dictionary<string, string>> GetGrantedPermissions()
    {
        var allPermissions = _permissionManager.GetExpandedPermissions()
            .Select(p => $"{WheelClaimTypes.Permission}.{p.Resource}.{p.Action}");
        var grantedPermissions = new List<string>();
        if (CurrentUser.IsAuthenticated())
        {
            foreach (var permission in allPermissions)
            {
                if (await UserService.HasPermissionAsync(CurrentUser.GetUserId(),
                        permission))
                {
                    grantedPermissions.Add(permission);
                }
            }
        }

        return grantedPermissions.ToDictionary(permission => permission, permission => "true");
    }

    private bool HasPermission(string resource, string action)
    {
        return AsyncHelper.RunSync(async () => await UserService.HasPermissionAsync(CurrentUser.GetUserId(),
            $"{WheelClaimTypes.Permission}.${resource}.${action}"));
    }

    private void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors();
    }
}