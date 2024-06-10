using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Controllers;
using pandx.Wheel.Folders;
using pandx.Wheel.MimeTypes;
using Sample.Application.Notifications;
using Sample.Application.Notifications.Dto;

namespace Sample.Host.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class NotificationsController : WheelControllerBase
{
    private readonly ICommonFolder _commonFolder;
    private readonly IMimeTypeManager _mimeTypeManager;
    private readonly INotificationAppService _notificationAppService;

    public NotificationsController(INotificationAppService notificationAppService, ICommonFolder commonFolder,
        IMimeTypeManager mimeTypeManager)
    {
        _notificationAppService = notificationAppService;
        _commonFolder = commonFolder;
        _mimeTypeManager = mimeTypeManager;
    }

    [HttpPost(Name = nameof(GetPagedUserNotifications))]
    public async Task<GetUserNotificationsResponse> GetPagedUserNotifications(GetUserNotificationsRequest request)
    {
        return await _notificationAppService.GetPagedUserNotificationsAsync(request);
    }

    [HttpPost(Name = nameof(SetUserNotificationAsRead))]
    public Task SetUserNotificationAsRead(EntityDto<Guid> request)
    {
        return _notificationAppService.SetUserNotificationAsReadAsync(request);
    }

    [HttpPost(Name = nameof(DeleteUserNotification))]
    public Task DeleteUserNotification(EntityDto<Guid> request)
    {
        return _notificationAppService.DeleteUserNotificationAsync(request);
    }

    [HttpGet(Name = nameof(Download))]
    public IActionResult Download(string fileName)
    {
        return File(new FileStream(Path.Combine(_commonFolder.FilesFolder, fileName!), FileMode.Open),
            _mimeTypeManager.GetMimeType(fileName!) ?? "application/octet-stream",
            fileName);
    }
}