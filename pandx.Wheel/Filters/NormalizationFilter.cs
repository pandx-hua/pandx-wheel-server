using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using pandx.Wheel.Validation;

namespace pandx.Wheel.Filters;

public class NormalizationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is IShouldNormalize shouldNormalize)
                {
                    shouldNormalize.Normalize();
                }
            }
        }

        await next();
    }
}