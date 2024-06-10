using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace pandx.Wheel.DependencyInjection;

public static class DependencyInjectionExtensions
{
    private static IServiceCollection AddService(this IServiceCollection services, Type serviceType,
        Type implementationType, ServiceLifetime serviceLifetime)
    {
        return serviceLifetime switch
        {
            ServiceLifetime.Transient => services.Transient(serviceType, implementationType),
            ServiceLifetime.Scoped => services.Scoped(serviceType, implementationType),
            ServiceLifetime.Singleton => services.Singleton(serviceType, implementationType),
            _ => throw new ArgumentException("错误的生命周期", nameof(serviceLifetime))
        };
    }

    private static IServiceCollection Transient(this IServiceCollection services, Type serviceType,
        Type implementationType)
    {
        services.AddTransient(serviceType,
            serviceProvider => ImplementationInstance(implementationType, serviceProvider));
        return services;
    }

    private static IServiceCollection Scoped(this IServiceCollection services, Type serviceType,
        Type implementationType)
    {
        services.AddScoped(serviceType, serviceProvider => ImplementationInstance(implementationType, serviceProvider));
        return services;
    }

    private static IServiceCollection Singleton(this IServiceCollection services, Type serviceType,
        Type implementationType)
    {
        services.AddSingleton(serviceType,
            serviceProvider => ImplementationInstance(implementationType, serviceProvider));
        return services;
    }

    private static object ImplementationInstance(Type implementationType, IServiceProvider serviceProvider)
    {
        var implementationInstance = ActivatorUtilities.CreateInstance(serviceProvider, implementationType);
        switch (implementationInstance)
        {
            case ServiceBase serviceBase:
                foreach (var property in serviceBase.GetType()
                             .GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var attr = property.GetCustomAttribute<InjectionAttribute>();
                    if (attr is not null)
                    {
                        property.SetValue(serviceBase, serviceProvider.GetRequiredService(property.PropertyType));
                    }
                }

                break;
        }

        return implementationInstance;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, Assembly assembly,
        Type dependencyType, ServiceLifetime serviceLifetime)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && dependencyType.IsAssignableFrom(t))
            .Select(t => new
            {
                Serivces = t.GetInterfaces().Where(i => !i.Name.EndsWith("Dependency")).Concat(new[] { t }),
                Implementation = t
            })
            .Where(t => t.Serivces.Any());
        foreach (var type in types)
        {
            foreach (var service in type.Serivces)
            {
                if (!dependencyType.IsAssignableFrom(service))
                {
                    continue;
                }

                //泛型类型的注册，可以正常注册，但运行时无法获取实现实例，Why？
                // if (service.IsGenericType)
                // {
                //     //泛型类型的注册
                //     if (!type.Implementation.IsGenericType)
                //     {
                //         continue;
                //     }
                //     if (service.GenericTypeArguments.Length != type.Implementation.GenericTypeArguments.Length)
                //     {
                //         continue;
                //     }
                //
                //     services.AddService(service.GetGenericTypeDefinition(),
                //         type.Implementation, serviceLifetime);
                // }
                // else
                // {
                //非泛型类型的注册
                services.AddService(service, type.Implementation, serviceLifetime);
                // }
            }
        }

        return services;
    }

    public static IServiceCollection AddServicesByConvention(this IServiceCollection services, Assembly assembly)
    {
        services
            .AddServices(assembly, typeof(ITransientDependency), ServiceLifetime.Transient)
            .AddServices(assembly, typeof(IScopedDependency), ServiceLifetime.Scoped)
            .AddServices(assembly, typeof(ISingletonDependency), ServiceLifetime.Singleton);

        return services;
    }
}