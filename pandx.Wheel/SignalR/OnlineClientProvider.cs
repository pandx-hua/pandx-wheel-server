using Microsoft.AspNetCore.SignalR;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization;
using pandx.Wheel.Miscellaneous;

namespace pandx.Wheel.SignalR;

public class OnlineClientProvider : IOnlineClientProvider
{
    private readonly IClientInfoProvider _clientInfoProvider;

    public OnlineClientProvider(IClientInfoProvider clientInfoProvider)
    {
        _clientInfoProvider = clientInfoProvider;
    }

    public IOnlineClient CreateClientForCurrentConnection(HubCallerContext context)
    {
        return new OnlineClient(context.ConnectionId, _clientInfoProvider.ClientIpAddress ?? string.Empty,
            context.User!.GetUserId());
    }
}