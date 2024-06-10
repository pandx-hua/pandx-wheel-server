using pandx.Wheel.Authorization.Roles;
using pandx.Wheel.Domain.Entities;

namespace Sample.Domain.Authorization.Roles;

public class ApplicationRole : WheelRole, ISoftDelete
{
    public bool IsDeleted { get; set; }
}