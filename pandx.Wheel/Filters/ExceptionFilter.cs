using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using pandx.Wheel.Auditing;

namespace pandx.Wheel.Filters;

public class ExceptionFilter : IAsyncExceptionFilter
{
    private readonly IAuditingHelper _auditingHelper;
    private readonly AuditingSettings _auditingSettings;

    public ExceptionFilter(
        IAuditingHelper auditingHelper,
        IOptions<AuditingSettings> auditingSettings)
    {
        _auditingSettings = auditingSettings.Value;
        _auditingHelper = auditingHelper;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor)
        {
            if (_auditingHelper.AuditingInfo is { } auditingInfo && _auditingHelper.Stopwatch is { } stopwatch)
            {
                stopwatch.Stop();
                auditingInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                if (_auditingSettings.SaveExceptions)
                {
                    auditingInfo.Exception = JsonConvert.SerializeObject(context.Exception);
                }

                await _auditingHelper.Save(auditingInfo);
            }
        }
    }
}