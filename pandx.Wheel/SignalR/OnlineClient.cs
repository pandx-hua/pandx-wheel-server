using pandx.Wheel.Extensions;

namespace pandx.Wheel.SignalR;

[Serializable]
public class OnlineClient : IOnlineClient
{
    private Dictionary<string, object> _properties = new();

    public OnlineClient()
    {
        ConnectTime = DateTime.Now;
    }

    public OnlineClient(string connectionId, string ipAddress, Guid userId) : this()
    {
        ConnectionId = connectionId;
        IpAddress = ipAddress;
        UserId = userId;
    }

    public string ConnectionId { get; set; } = default!;
    public string IpAddress { get; set; } = default!;
    public Guid? UserId { get; set; }
    public DateTime ConnectTime { get; set; }

    public Dictionary<string, object> Properties
    {
        get => _properties;
        set => _properties = value ?? throw new ArgumentNullException(nameof(value));
    }

    public object this[string key]
    {
        get => Properties[key];
        set => Properties[key] = value;
    }

    public override string ToString()
    {
        return this.ToJsonString();
    }
}