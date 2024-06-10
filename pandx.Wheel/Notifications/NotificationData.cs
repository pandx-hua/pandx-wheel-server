using pandx.Wheel.Extensions;

namespace pandx.Wheel.Notifications;

[Serializable]
public class NotificationData
{
    public NotificationData()
    {
        Properties = new Dictionary<string, object>();
    }

    public object this[string key]
    {
        get => Properties.GetOrDefault(key);
        set => Properties[key] = value;
    }

    public Dictionary<string, object> Properties { get; set; }

    public override string ToString()
    {
        return this.ToJsonString();
    }
}