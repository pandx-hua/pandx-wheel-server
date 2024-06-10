using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Events;
using Sample.Domain.Authorization.Users;

namespace Sample.Application.Authorization.Users;

public class UserCreatedEventHandler : NotificationEventHandler<UserCreatedEvent<ApplicationUser>>
{
    protected override Task Handle(UserCreatedEvent<ApplicationUser> @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"用户创建成功了 {@event.User.Name} ");
        //TODO
        return Task.CompletedTask;
        ;
    }
}