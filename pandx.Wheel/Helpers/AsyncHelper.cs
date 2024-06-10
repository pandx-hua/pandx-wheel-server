﻿using System.Reflection;
using Nito.AsyncEx;

namespace pandx.Wheel.Helpers;

public static class AsyncHelper
{
    public static bool IsAsync(this MethodInfo method)
    {
        return method.ReturnType == typeof(Task) || (method.ReturnType.GetTypeInfo().IsGenericType &&
                                                     method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
    }

    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        return AsyncContext.Run(func);
    }

    public static void RunSync(Func<Task> action)
    {
        AsyncContext.Run(action);
    }
}