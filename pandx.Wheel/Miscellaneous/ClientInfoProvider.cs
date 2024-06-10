using Microsoft.AspNetCore.Http;

namespace pandx.Wheel.Miscellaneous;

public class ClientInfoProvider : IClientInfoProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? BrowserInfo => GetBrowserInfo();
    public string? ClientIpAddress => GetClientIpAddress();
    public string? ComputerName => GetComputerName();

    public string? GetBrowserInfo()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.Request.Headers?["User-Agent"];
    }

    public string? GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.Connection.RemoteIpAddress?.ToString();
    }

    public string? GetComputerName()
    {
        return null;
    }
}