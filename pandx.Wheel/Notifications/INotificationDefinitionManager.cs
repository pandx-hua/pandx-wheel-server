using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Notifications;

public interface INotificationDefinitionManager : ISingletonDependency
{
    Task InitializeAsync();
    void Add(NotificationDefinition notificationDefinition);
    NotificationDefinition? Get(string name);
    void Remove(string name);
    IReadOnlyList<NotificationDefinition> GetAll();
}