using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace pandx.Wheel.Modules;

public static class ModuleExtensions
{
    public static WebApplicationBuilder InitializeModules(this WebApplicationBuilder builder,
        IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            var moduleTypes = types.Where(t =>
                t is { IsClass: true, IsAbstract: false } && typeof(IModule).IsAssignableFrom(t));
            foreach (var moduleType in moduleTypes)
            {
                var module = Activator.CreateInstance(moduleType);
                _ = module ?? throw new Exception($"无法创建 {moduleType} 模块");


                ((IModule)module).Initialize(builder);
            }
        }

        return builder;
    }
}