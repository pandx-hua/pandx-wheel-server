using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Auditing;

public interface IAuditingHelper : IScopedDependency
{
    Stopwatch Stopwatch { get; set; }
    AuditingInfo AuditingInfo { get; set; }
    bool ShouldSave(ActionExecutingContext context);
    AuditingInfo CreateAuditing(ActionExecutingContext context);
    AuditingInfo CreateAuditing(ResultExecutingContext context);
    Task Save(AuditingInfo auditingInfo);
}