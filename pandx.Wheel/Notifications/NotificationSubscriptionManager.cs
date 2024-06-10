using pandx.Wheel.Authorization;

namespace pandx.Wheel.Notifications;

public class NotificationSubscriptionManager : INotificationSubscriptionManager
{
    private readonly INotificationDefinitionManager _notificationDefinitionManager;
    private readonly INotificationManager _notificationManager;

    public NotificationSubscriptionManager(INotificationManager notificationManager,
        INotificationDefinitionManager notificationDefinitionManager)
    {
        _notificationManager = notificationManager;
        _notificationDefinitionManager = notificationDefinitionManager;
    }

    public async Task SubscribeAsync(UserIdentifier userIdentifier, string notificationName)
    {
        if (await IsSubscribedAsync(userIdentifier, notificationName))
        {
            return;
        }

        await _notificationManager.InsertNotificationSubscriptionAsync(new NotificationSubscription
        {
            UserId = userIdentifier.UserId,
            NotificationName = notificationName
        });
    }

    public Task<bool> IsSubscribedAsync(UserIdentifier userIdentifier, string notificationName)
    {
        return _notificationManager.IsSubscribedAsync(userIdentifier, notificationName);
    }

    public async Task SubscribeToAllNotificationsAsync(UserIdentifier userIdentifier)
    {
        var notificationDefinitions = _notificationDefinitionManager.GetAll().ToList();
        foreach (var notificationDefinition in notificationDefinitions)
        {
            await SubscribeAsync(userIdentifier, notificationDefinition.Name);
        }
    }
}