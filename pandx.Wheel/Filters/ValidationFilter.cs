using FluentValidation;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using pandx.Wheel.Validation;

namespace pandx.Wheel.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is IShouldValidate shouldValidate)
                {
                    if (context.HttpContext.RequestServices.GetService(
                            typeof(IValidator<>).MakeGenericType(shouldValidate.GetType())) is IValidator validator)
                    {
                        var validationResult = await validator.ValidateAsync(
                            new ValidationContext<object>(argument.Value),
                            context.HttpContext.RequestAborted);
                        if (!validationResult.IsValid)
                        {
                            throw new ValidationException("FluentValidation验证失败", validationResult.Errors);
                        }
                    }
                }
            }
        }

        await next();
    }
}