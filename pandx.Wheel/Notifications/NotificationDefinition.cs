using pandx.Wheel.Extensions;

namespace pandx.Wheel.Notifications;

public class NotificationDefinition
{
    public NotificationDefinition(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "NotificationName 不能为空");
        }

        Name = name;
        Description = description;
        Properties = new Dictionary<string, object>();
    }

    public string Name { get; set; }
    public string Description { get; set; }

    public IDictionary<string, object> Properties { get; set; }

    public object this[string key]
    {
        get => Properties.GetOrDefault(key);
        set => Properties[key] = value;
    }
}