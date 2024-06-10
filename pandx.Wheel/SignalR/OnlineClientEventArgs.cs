namespace pandx.Wheel.SignalR;

public class OnlineClientEventArgs : EventArgs
{
    public OnlineClientEventArgs(IOnlineClient client)
    {
        Client = client;
    }

    public IOnlineClient Client { get; }
}