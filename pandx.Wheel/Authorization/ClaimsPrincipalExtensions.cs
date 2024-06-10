using System.Security.Claims;

namespace pandx.Wheel.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(WheelClaimTypes.UserIdentifier) is not null
            ? Guid.Parse(principal.FindFirstValue(WheelClaimTypes.UserIdentifier)!)
            : Guid.Empty;
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(WheelClaimTypes.Email);
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(WheelClaimTypes.UserName);
    }

    public static string? GetName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(WheelClaimTypes.TrueName)?.Value;
    }
}