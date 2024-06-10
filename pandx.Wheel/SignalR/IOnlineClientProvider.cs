using Microsoft.AspNetCore.SignalR;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.SignalR;

public interface IOnlineClientProvider : ITransientDependency
{
    IOnlineClient CreateClientForCurrentConnection(HubCallerContext context);
}