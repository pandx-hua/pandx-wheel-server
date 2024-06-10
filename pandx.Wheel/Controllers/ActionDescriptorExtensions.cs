using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace pandx.Wheel.Controllers;

public static class ActionDescriptorExtensions
{
    public static bool IsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
    {
        return actionDescriptor is ControllerActionDescriptor;
    }

    public static MethodInfo GetMethodInfo(this ActionDescriptor actionDescriptor)
    {
        return actionDescriptor.AsControllerActionDescriptor().MethodInfo;
    }

    public static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
    {
        if (!actionDescriptor.IsControllerActionDescriptor())
        {
            throw new Exception(
                $"{nameof(actionDescriptor)}应是{typeof(ControllerActionDescriptor).AssemblyQualifiedName}类型，但这里不是");
        }

        return (actionDescriptor as ControllerActionDescriptor)!;
    }
}