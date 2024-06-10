using pandx.Wheel.Authorization;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Notifications;

public interface INotificationManager : ITransientDependency
{
    //subscription
    Task InsertNotificationSubscriptionAsync(NotificationSubscription notificationSubscription);
    Task DeleteNotificationSubscriptionAsync(UserIdentifier userIdentifier, string notificationName);
    Task<List<NotificationSubscription>> GetNotificationSubscriptionsAsync(string notificationName);
    Task<List<NotificationSubscription>> GetNotificationSubscriptionsAsync(UserIdentifier userIdentifier);
    Task<bool> IsSubscribedAsync(UserIdentifier userIdentifier, string notificationName);

    //notification
    Task InsertNotificationAsync(Notification notification);
    Task<Notification?> GetNotificationAsync(Guid notificationId);
    Task DeleteNotificationAsync(Guid notificationId);

    //user notification
    Task InsertUserNotificationAsync(UserNotification userNotification);

    Task UpdateUserNotificationStateAsync(UserIdentifier userIdentifier, UserNotificationState state,
        Guid userNotificationId);

    Task UpdateUserNotificationsStateAsync(UserIdentifier userIdentifier, UserNotificationState state);
    Task DeleteUserNotificationAsync(UserIdentifier userIdentifier, Guid userNotificationId);

    Task DeleteUserNotificationsAsync(UserIdentifier userIdentifier, UserNotificationState? state = null,
        DateTime? startTime = null,
        DateTime? endTime = null);

    Task<UserNotification?> GetUserNotificationAsync(UserIdentifier userIdentifier, Guid userNotificationId);

    Task<List<UserNotification>> GetUserNotificationsAsync(UserIdentifier userIdentifier,
        UserNotificationState? state = null,
        int currentPage = 1, int pageSize = int.MaxValue, DateTime? startTime = null, DateTime? endTime = null);

    Task<int> GetUserNotificationCountAsync(UserIdentifier userIdentifier, UserNotificationState? state = null,
        DateTime? startTime = null, DateTime? endTime = null);
}