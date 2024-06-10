using pandx.Wheel.Events;

namespace pandx.Wheel.Authorization.Users;

public class UserCreatedEvent<TUser> : Event where TUser : WheelUser
{
    public UserCreatedEvent(TUser user)
    {
        User = user;
    }

    public TUser User { get; set; }
}

public class UserUpdatedEvent<TUser> : Event where TUser : WheelUser
{
    public UserUpdatedEvent(TUser user)
    {
        User = user;
    }

    public TUser User { get; set; }
}

public class UserDeletedEvent<TUser> : Event where TUser : WheelUser
{
    public UserDeletedEvent(TUser user)
    {
        User = user;
    }

    public TUser User { get; set; }
}