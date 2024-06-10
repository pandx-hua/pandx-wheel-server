using Microsoft.AspNetCore.Builder;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Modules;

public class Module : IModule
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddServicesByConvention(typeof(Module).Assembly);
    }
}