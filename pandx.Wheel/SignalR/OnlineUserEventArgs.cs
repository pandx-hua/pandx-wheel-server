using pandx.Wheel.Authorization;

namespace pandx.Wheel.SignalR;

public class OnlineUserEventArgs : OnlineClientEventArgs
{
    public OnlineUserEventArgs(UserIdentifier user, IOnlineClient client) : base(client)
    {
        User = user;
    }

    public UserIdentifier User { get; }
}