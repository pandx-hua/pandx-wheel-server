using pandx.Wheel.Events;

namespace pandx.Wheel.Authorization.Roles;

public class RoleCreatedEvent<TRole> : Event where TRole : WheelRole
{
    public RoleCreatedEvent(TRole role)
    {
        Role = role;
    }

    public TRole Role { get; set; }
}

public class RoleUpdatedEvent<TRole> : Event where TRole : WheelRole
{
    public RoleUpdatedEvent(TRole role)
    {
        Role = role;
    }

    public TRole Role { get; set; }
}

public class RoleDeletedEvent<TRole> : Event where TRole : WheelRole
{
    public RoleDeletedEvent(TRole role)
    {
        Role = role;
    }

    public TRole Role { get; set; }
}