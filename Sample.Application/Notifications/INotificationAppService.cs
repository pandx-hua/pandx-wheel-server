using pandx.Wheel.Application.Dto;
using pandx.Wheel.Application.Services;
using Sample.Application.Notifications.Dto;

namespace Sample.Application.Notifications;

public interface INotificationAppService : IApplicationService
{
    Task<int> GetUserNotificationCount(GetUserNotificationCountRequest request);

    Task<GetUserNotificationsResponse> GetPagedUserNotificationsAsync(GetUserNotificationsRequest request);
    Task SetUserNotificationAsReadAsync(EntityDto<Guid> request);
    Task DeleteUserNotificationAsync(EntityDto<Guid> request);
}