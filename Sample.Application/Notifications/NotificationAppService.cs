using pandx.Wheel.Application.Dto;
using pandx.Wheel.Authorization;
using pandx.Wheel.Notifications;
using Sample.Application.Notifications.Dto;

namespace Sample.Application.Notifications;

public class NotificationAppService : SampleAppServiceBase, INotificationAppService
{
    private readonly INotificationManager _notificationManager;

    public NotificationAppService(INotificationManager notificationManager)
    {
        _notificationManager = notificationManager;
    }

    public async Task<int> GetUserNotificationCount(GetUserNotificationCountRequest request)
    {
        return await _notificationManager.GetUserNotificationCountAsync(
            new UserIdentifier(CurrentUser.GetUserId().ToString()),
            request.State, request.StartTime, request.EndTime);
    }

    public async Task<GetUserNotificationsResponse> GetPagedUserNotificationsAsync(GetUserNotificationsRequest request)
    {
        var unReadCount = await _notificationManager.GetUserNotificationCountAsync(
            new UserIdentifier(CurrentUser.GetUserId().ToString()), 0, request.StartTime, request.EndTime);
        var totalCount = await _notificationManager.GetUserNotificationCountAsync(
            new UserIdentifier(CurrentUser.GetUserId().ToString()), request.State, request.StartTime, request.EndTime);
        var userNotifications = await _notificationManager.GetUserNotificationsAsync(
            new UserIdentifier(CurrentUser.GetUserId().ToString()), request.State, request.CurrentPage,
            request.PageSize, request.StartTime, request.EndTime);

        var dtos = userNotifications.Select(item =>
        {
            var dto = Mapper.Map<UserNotificationsDto>(item);
            return dto;
        }).ToList();
        return new GetUserNotificationsResponse(totalCount, unReadCount, dtos);
    }

    public Task SetUserNotificationAsReadAsync(EntityDto<Guid> request)
    {
        return _notificationManager.UpdateUserNotificationStateAsync(
            new UserIdentifier(CurrentUser.GetUserId().ToString()), UserNotificationState.Read, request.Id);
    }

    public Task DeleteUserNotificationAsync(EntityDto<Guid> request)
    {
        return _notificationManager.DeleteUserNotificationAsync(new UserIdentifier(CurrentUser.GetUserId().ToString()),
            request.Id);
    }
}