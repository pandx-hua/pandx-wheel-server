using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace pandx.Wheel.Helpers;

public static class ResultHelper
{
    public static bool IsObjectResult(Type returnType)
    {
        if (returnType == typeof(Task))
        {
            returnType = typeof(void);
        }
        else if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            returnType = returnType.GenericTypeArguments[0];
        }

        if (typeof(IActionResult).GetTypeInfo().IsAssignableFrom(returnType))
        {
            if (typeof(JsonResult).GetTypeInfo().IsAssignableFrom(returnType) ||
                typeof(ObjectResult).GetTypeInfo().IsAssignableFrom(returnType))
            {
                return true;
            }

            return false;
        }

        return true;
    }
}