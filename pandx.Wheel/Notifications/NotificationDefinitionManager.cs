using System.Collections.Immutable;
using pandx.Wheel.Extensions;

namespace pandx.Wheel.Notifications;

public class NotificationDefinitionManager : INotificationDefinitionManager
{
    private readonly IDictionary<string, NotificationDefinition> _notificationDefinitions;
    private readonly INotificationProvider _notificationProvider;

    public NotificationDefinitionManager(INotificationProvider notificationProvider)
    {
        _notificationDefinitions = new Dictionary<string, NotificationDefinition>();
        _notificationProvider = notificationProvider;
    }

    public async Task InitializeAsync()
    {
        var context = new NotificationDefinitionContext(this);
        await _notificationProvider.SetNotificationsAsync(context);
    }

    public void Add(NotificationDefinition notificationDefinition)
    {
        if (_notificationDefinitions.ContainsKey(notificationDefinition.Name))
        {
            throw new Exception("已经包含了");
        }

        _notificationDefinitions[notificationDefinition.Name] = notificationDefinition;
    }

    public NotificationDefinition? Get(string name)
    {
        return _notificationDefinitions.GetOrDefault(name);
    }

    public void Remove(string name)
    {
        _notificationDefinitions.Remove(name);
    }

    public IReadOnlyList<NotificationDefinition> GetAll()
    {
        return _notificationDefinitions.Values.ToImmutableList();
    }
}