using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using pandx.Wheel.Domain.UnitOfWork;

namespace pandx.Wheel.Persistence.EntityFrameworkCore;

public class EfCoreUnitOfWorkFilter : IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork;

    public EfCoreUnitOfWorkFilter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var notAutoCommit = false;
        if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            notAutoCommit = descriptor.MethodInfo.IsDefined(typeof(NoAutoCommitAttribute), true);
        }

        if (notAutoCommit)
        {
            await next();
            return;
        }

        if ((await next()).Exception is null)
        {
            await _unitOfWork.CommitAsync();
        }
    }
}