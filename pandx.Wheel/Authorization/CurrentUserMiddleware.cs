using Microsoft.AspNetCore.Http;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Authorization;

public class CurrentUserMiddleware : IMiddleware, IScopedDependency
{
    private readonly ICurrentUser _currentUser;

    public CurrentUserMiddleware(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _currentUser.SetCurrentUser(context.User);
        await next(context);
    }
}