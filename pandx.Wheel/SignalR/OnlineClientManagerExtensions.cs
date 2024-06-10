using pandx.Wheel.Authorization;

namespace pandx.Wheel.SignalR;

public static class OnlineClientManagerExtensions
{
    public static bool IsOnline(this IOnlineClientManager onlineClientManager, UserIdentifier userIdentifier)
    {
        return onlineClientManager.GetAllByUser(userIdentifier).Any();
    }

    public static bool Remove(this IOnlineClientManager onlineClientManager, IOnlineClient client)
    {
        return onlineClientManager.Remove(client.ConnectionId);
    }
}