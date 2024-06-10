using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using pandx.Wheel.Authorization;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.SignalR;

namespace pandx.Wheel.Notifications;

public class NotificationDistributor : ServiceBase, INotificationDistributor
{
    private readonly ILogger<NotificationDistributor> _logger;
    private readonly INotificationManager _notificationManager;
    private readonly IRealTimeNotifier _realTimeNotifier;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationDistributor(INotificationManager notificationManager, IUnitOfWork unitOfWork,
        ILogger<NotificationDistributor> logger,
        IRealTimeNotifier realTimeNotifier)
    {
        _notificationManager = notificationManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _realTimeNotifier = realTimeNotifier;
    }

    public async Task DistributeAsync(Guid notificationId)
    {
        var notification = await _notificationManager.GetNotificationAsync(notificationId);
        if (notification is null)
        {
            _logger.LogWarning("未发现Id为 {NotificationId} 的Notification，NotificationDistributionJob 无法继续",
                notificationId);
            return;
        }

        var users = await GetUsersAsync(notification);
        var userNotifications = await SaveUserNotificationsAsync(users, notification);
        await _notificationManager.DeleteNotificationAsync(notification.Id);
        await NotifyAsync(userNotifications.ToArray());
    }

    public async Task<UserIdentifier[]> GetUsersAsync(Notification notification)
    {
        List<UserIdentifier> userIdentifiers;
        if (!notification.UserIds.IsNullOrEmpty())
        {
            userIdentifiers = notification.UserIds!.Split(",").Select(x => new UserIdentifier(x)).ToList();
        }
        else
        {
            var subscriptions =
                await _notificationManager.GetNotificationSubscriptionsAsync(notification.NotificationName);
            userIdentifiers = subscriptions.Select(x => new UserIdentifier(x.UserId.ToString())).ToList();
        }

        if (!notification.ExcludedUserIds.IsNullOrEmpty())
        {
            var excludedUserIds = notification.ExcludedUserIds!.Split(",").Select(x => new UserIdentifier(x)).ToList();
            userIdentifiers.RemoveAll(x => excludedUserIds.Any(y => y.Equals(x)));
        }

        return userIdentifiers.ToArray();
    }

    public async Task<List<UserNotification>> SaveUserNotificationsAsync(UserIdentifier[] userIdentifiers,
        Notification notification)
    {
        var userNotifications = new List<UserNotification>();
        foreach (var userIdentifier in userIdentifiers)
        {
            var userNotification = new UserNotification
            {
                UserId = userIdentifier.UserId,
                NotificationName = notification.NotificationName,
                NotificationData = notification.NotificationData,
                Severity = notification.Severity
            };
            await _notificationManager.InsertUserNotificationAsync(userNotification);
            userNotifications.Add(userNotification);
        }

        return userNotifications;
    }

    public async Task NotifyAsync(UserNotification[] userNotifications)
    {
        await _realTimeNotifier.SendNotificationsAsync(userNotifications);
    }
}