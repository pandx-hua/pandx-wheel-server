using Microsoft.Extensions.Logging;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.SignalR;

public class OnlineClientHub : HubBase, ITransientDependency
{
    private readonly ILogger<OnlineClientHub> _logger;
    private readonly IOnlineClientManager _onlineClientManager;
    private readonly IOnlineClientProvider _onlineClientProvider;

    public OnlineClientHub(IOnlineClientManager onlineClientManager, IOnlineClientProvider onlineClientProvider,
        ILogger<OnlineClientHub> logger)
    {
        _onlineClientManager = onlineClientManager;
        _onlineClientProvider = onlineClientProvider;
        _logger = logger;
    }


    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        var client = _onlineClientProvider.CreateClientForCurrentConnection(Context);
        _logger.LogDebug("A client is connected: " + client);
        _onlineClientManager.Add(client);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        _logger.LogDebug("A client is disconnected: " + Context.ConnectionId);
        _onlineClientManager.Remove(Context.ConnectionId);
    }
}