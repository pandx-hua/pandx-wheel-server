using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Authorization.Users;

public abstract class WheelUser : IdentityUser<Guid>, IAudited, IEntity<Guid>
{
    public string Name { get; set; } = default!;
    public Gender Gender { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationUtcTime { get; set; }
    public bool IsActive { get; set; }
    public string? OpenId { get; set; }
    public Guid Creator { get; set; }
    public Guid? Deleter { get; set; }
    public Guid? Modifier { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? DeletionTime { get; set; }
    public DateTime? ModificationTime { get; set; }
}

public enum Gender
{
    [Description("男")] Male = 0,
    [Description("女")] Female = 1
}