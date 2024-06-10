using Microsoft.AspNetCore.Builder;
using pandx.Wheel.Authorization;
using Serilog;

namespace pandx.Wheel.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseWheel(this WebApplication app)
    {
        //before such as MVC and after UseStaticFiles
        app.UseSerilogRequestLogging();
        app.UseMiddleware<CurrentUserMiddleware>();
        return app;
    }
}