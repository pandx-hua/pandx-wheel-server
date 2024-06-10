namespace pandx.Wheel.SignalR;

public interface IOnlineClient
{
    string ConnectionId { get; set; }
    string IpAddress { get; set; }
    Guid? UserId { get; set; }
    DateTime ConnectTime { get; set; }
    Dictionary<string, object> Properties { get; set; }
    object this[string key] { get; set; }
}