using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Authorization.Logins;

public interface ILoginService:ITransientDependency
{
    Task CreateLoginAttemptAsync(string userNameOrEmail,LoginResultType loginResultType,Guid? userId=null);
}