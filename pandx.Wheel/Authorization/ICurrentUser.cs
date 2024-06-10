using System.Security.Claims;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Authorization;

public interface ICurrentUser : IScopedDependency
{
    string? GetUserName();
    Guid GetUserId();
    string? GetEmail();
    string? GetName();
    bool IsAuthenticated();
    bool IsInRole(string role);
    IEnumerable<Claim>? GetUserClaims();
    void SetCurrentUser(ClaimsPrincipal user);
    void SetCurrentUserId(string userId);
}