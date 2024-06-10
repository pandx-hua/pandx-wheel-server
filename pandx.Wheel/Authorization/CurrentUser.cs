using System.Security.Claims;

namespace pandx.Wheel.Authorization;

public class CurrentUser : ICurrentUser
{
    private ClaimsPrincipal? _user;
    private Guid _userId = Guid.Empty;

    public Guid GetUserId()
    {
        return IsAuthenticated() ? _user?.GetUserId() ?? Guid.Empty : _userId;
    }

    public string? GetEmail()
    {
        return IsAuthenticated() ? _user?.GetEmail() : string.Empty;
    }

    public string? GetUserName()
    {
        return IsAuthenticated() ? _user?.GetUserName() : string.Empty;
    }

    public string? GetName()
    {
        return IsAuthenticated() ? _user?.GetName() : string.Empty;
    }

    public bool IsAuthenticated()
    {
        return _user?.Identity?.IsAuthenticated is true;
    }

    public bool IsInRole(string role)
    {
        return _user?.IsInRole(role) is true;
    }

    public IEnumerable<Claim>? GetUserClaims()
    {
        return _user?.Claims;
    }

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user is not null)
        {
            throw new Exception("存在用户？");
        }

        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        if (_userId != Guid.Empty)
        {
            throw new Exception("存在用户？");
        }

        if (!string.IsNullOrEmpty(userId))
        {
            _userId = Guid.Parse(userId);
        }
    }
}