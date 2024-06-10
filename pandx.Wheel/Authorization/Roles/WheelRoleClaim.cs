using Microsoft.AspNetCore.Identity;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Authorization.Roles;

public abstract class WheelRoleClaim : IdentityRoleClaim<Guid>, IAudited
{
    public Guid Creator { get; set; }
    public Guid? Deleter { get; set; }
    public Guid? Modifier { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? DeletionTime { get; set; }
    public DateTime? ModificationTime { get; set; }
}