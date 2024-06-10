using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using pandx.Wheel.Validation;

namespace pandx.Wheel.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        // _logger.LogError(exception, exception.Message);

        var details = new MyProblemDetails
        {
            Instance = httpContext.Request.Path
        };
        if (exception is AggregateException { InnerExceptions: { } innerExceptions })
        {
            exception = innerExceptions.First();
        }

        if (exception is ClientException clientException)
        {
            switch (clientException.ErrorCode)
            {
                case 400:
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case 401:
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                case 404:
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
                case 409:
                    httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                    break;
                case 422:
                    httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    break;
            }

            details.Title = clientException.Message;
            details.Detail = clientException.Details;
            details.Status = clientException.ErrorCode;
            await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);
            return true;
        }

        if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = 484;
            details.Title = validationException.Message;
            details.Status = 484;
            details.ValidationErrors = GetValidationErrors(validationException);
            await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);
            return true;
        }

        if (exception is WheelException wheelException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            details.Title = wheelException.Message;
            details.Status = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);
            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        details.Title = exception.Message;
        details.Status = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);
        return true;
    }

    private List<ValidationError> GetValidationErrors(ValidationException exception)
    {
        var errors = new List<ValidationError>();
        foreach (var failure in exception.Errors)
        {
            var error = new ValidationError { Message = failure.ErrorMessage };
            errors.Add(error);
        }

        return errors;
    }
}