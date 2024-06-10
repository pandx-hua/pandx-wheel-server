using Microsoft.AspNetCore.Identity;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Domain.Entities;
using pandx.Wheel.Organizations;

namespace Sample.Domain.Authorization.Users;

public sealed class ApplicationUser : WheelUser, ISoftDelete
{
    public ApplicationUser()
    {
        LockoutEnabled = true;
        PhoneNumber = "";
    }

    public List<UserOrganization> Organizations { get; set; } = default!;
    public List<IdentityUserRole<Guid>> Roles { get; set; } = default!;

    public bool IsDeleted { get; set; }
    public Guid? AvatarId { get; set; }

    public void Unlock()
    {
        LockoutEnd = null;
        AccessFailedCount = 0;
    }
}