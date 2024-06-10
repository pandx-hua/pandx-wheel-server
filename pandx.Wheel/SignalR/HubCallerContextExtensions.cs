using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace pandx.Wheel.SignalR;

public static class HubCallerContextExtensions
{
    public static string GetUserId(this HubCallerContext context)
    {
        return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }
}