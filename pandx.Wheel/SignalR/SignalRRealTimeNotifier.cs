using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using pandx.Wheel.Notifications;

namespace pandx.Wheel.SignalR;

public class SignalRRealTimeNotifier : IRealTimeNotifier
{
    private readonly IHubContext<OnlineClientHub> _hubContext;
    private readonly ILogger<SignalRRealTimeNotifier> _logger;
    private readonly IOnlineClientManager _onlineClientManager;

    public SignalRRealTimeNotifier(IOnlineClientManager onlineClientManager, IHubContext<OnlineClientHub> hubContext,
        ILogger<SignalRRealTimeNotifier> logger)
    {
        _onlineClientManager = onlineClientManager;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNotificationsAsync(UserNotification[] userNotifications)
    {
        foreach (var userNotification in userNotifications)
        {
            var onlineClients = _onlineClientManager.GetAllByUser(userNotification);
            foreach (var onlineClient in onlineClients)
            {
                var signalRClient = _hubContext.Clients.Client(onlineClient.ConnectionId);
                await signalRClient.SendAsync("getNotification", userNotification);
            }
        }
    }
}