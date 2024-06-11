using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Controllers;

public class InjectionControllerActivator : IControllerActivator
{
    public object Create(ControllerContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));
        var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
        var controller = context.HttpContext.RequestServices.GetRequiredService(controllerType);
        if (controller is WheelControllerBase controllerBase)
        {
            foreach (var property in controllerBase.GetType()
                         .GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = property.GetCustomAttribute<InjectionAttribute>();
                if (attr is not null)
                {
                    property.SetValue(controllerBase,
                        context.HttpContext.RequestServices.GetRequiredService(property.PropertyType));
                }
            }
        }

        return controller;
    }

    public void Release(ControllerContext context, object controller)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));
        _ = controller ?? throw new ArgumentNullException(nameof(controller));


        if (controller is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}