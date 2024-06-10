using Microsoft.AspNetCore.Identity;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Authorization.Roles;

public abstract class WheelRole : IdentityRole<Guid>, IAudited, IEntity<Guid>
{
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsStatic { get; set; }
    public bool IsDefault { get; set; }
    public Guid Creator { get; set; }
    public Guid? Deleter { get; set; }
    public Guid? Modifier { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? DeletionTime { get; set; }
    public DateTime? ModificationTime { get; set; }
}