using Microsoft.AspNetCore.Builder;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Modules;

namespace Sample.Domain;

public class DomainModule : IModule
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddServicesByConvention(typeof(DomainModule).Assembly);
    }
}