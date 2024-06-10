using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using pandx.Wheel.Auditing;

namespace pandx.Wheel.Filters;

public class ActionFilter : IAsyncActionFilter
{
    private readonly IAuditingHelper _auditingHelper;
    private readonly AuditingSettings _auditingSettings;

    public ActionFilter(IAuditingHelper auditingHelper, IOptions<AuditingSettings> auditingSettings)
    {
        _auditingHelper = auditingHelper;
        _auditingSettings = auditingSettings.Value;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor)
        {
            if (!_auditingHelper.ShouldSave(context))
            {
                await next();
            }
            else
            {
                _auditingHelper.AuditingInfo = _auditingHelper.CreateAuditing(context);
                _auditingHelper.Stopwatch = Stopwatch.StartNew();

                await next();
            }
        }
        else
        {
            await next();
        }
    }
}