using pandx.Wheel.Authorization;

namespace pandx.Wheel.SignalR;

public static class OnlineClientExtensions
{
    public static UserIdentifier? ToUserIdentifier(this IOnlineClient onlineClient)
    {
        return new UserIdentifier(onlineClient.UserId.ToString() ?? string.Empty);
    }
}