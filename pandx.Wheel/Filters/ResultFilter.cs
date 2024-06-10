using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using pandx.Wheel.Auditing;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Models;

namespace pandx.Wheel.Filters;

public class ResultFilter : IAsyncResultFilter
{
    private readonly IAuditingHelper _auditingHelper;
    private readonly AuditingSettings _auditingSettings;

    public ResultFilter(IAuditingHelper auditingHelper, IOptions<AuditingSettings> auditingSettings)
    {
        _auditingHelper = auditingHelper;
        _auditingSettings = auditingSettings.Value;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        switch (context)
        {
            case { Result: BadRequestObjectResult badRequestResult }:
                await SaveAuditing(context, badRequestResult.Value!);
                throw new ClientException("Bad Request", JsonConvert.SerializeObject(badRequestResult.Value), 400);

            case { Result: UnauthorizedObjectResult unauthorizedObjectResult }:
                await SaveAuditing(context, unauthorizedObjectResult.Value!);
                throw new ClientException("Unauthorized", JsonConvert.SerializeObject(unauthorizedObjectResult.Value),
                    401);

            case { Result: NotFoundObjectResult notFoundObjectResult }:
                await SaveAuditing(context, notFoundObjectResult.Value!);
                throw new ClientException("Not Found", JsonConvert.SerializeObject(notFoundObjectResult.Value), 404);

            case { Result: ConflictObjectResult conflictObjectResult }:
                await SaveAuditing(context, conflictObjectResult.Value!);
                throw new ClientException("Conflict", JsonConvert.SerializeObject(conflictObjectResult.Value), 409);

            case { Result: UnprocessableEntityObjectResult unprocessableEntityObjectResult }:
                await SaveAuditing(context, unprocessableEntityObjectResult.Value!);
                throw new ClientException("Unprocessable Entity",
                    JsonConvert.SerializeObject(unprocessableEntityObjectResult.Value), 422);
        }

        if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            if (descriptor.MethodInfo.IsDefined(typeof(NoPackageAttribute), true))
            {
                await SaveAuditingCrossAction(context.Result);
            }
            else
            {
                switch (context)
                {
                    case { Result: ObjectResult objectResult }:
                        objectResult.Value = new PackagedResponse
                        {
                            Result = objectResult.Value!,
                            Success = true
                        };
                        objectResult.DeclaredType = typeof(PackagedResponse);
                        await SaveAuditingCrossAction(objectResult.Value);
                        break;
                    case { Result: JsonResult jsonResult }:
                        jsonResult.Value = new PackagedResponse
                        {
                            Result = jsonResult.Value!,
                            Success = true
                        };
                        await SaveAuditingCrossAction(jsonResult.Value);
                        break;
                    case { Result: EmptyResult emptyResult }:
                        context.Result = new ObjectResult(new PackagedResponse
                        {
                            Success = true
                        });
                        await SaveAuditingCrossAction((context.Result as ObjectResult)?.Value!);
                        break;
                    default:
                        await SaveAuditingCrossAction(context.Result);
                        break;
                }
            }
        }

        await next();
    }

    private async Task SaveAuditingCrossAction(object result)
    {
        if (_auditingHelper.AuditingInfo is { } auditingInfo && _auditingHelper.Stopwatch is { } stopwatch)
        {
            stopwatch.Stop();
            auditingInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
            if (_auditingSettings.SaveReturnValues)
            {
                auditingInfo.ReturnValue = JsonConvert.SerializeObject(result);
            }

            await _auditingHelper.Save(auditingInfo);
        }
    }

    private async Task SaveAuditing(ResultExecutingContext context, object result)
    {
        var auditingInfo = _auditingHelper.CreateAuditing(context);
        if (_auditingSettings.SaveExceptions)
        {
            auditingInfo.Exception = JsonConvert.SerializeObject(result);
        }

        await _auditingHelper.Save(auditingInfo);
    }
}