using Microsoft.AspNetCore.Builder;

namespace pandx.Wheel.Modules;

public interface IModule
{
    public void Initialize(WebApplicationBuilder builder);
}