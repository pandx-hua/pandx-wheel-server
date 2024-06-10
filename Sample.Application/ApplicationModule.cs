using Microsoft.AspNetCore.Builder;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Modules;

namespace Sample.Application;

public class ApplicationModule : IModule
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddServicesByConvention(typeof(ApplicationModule).Assembly);
    }
}