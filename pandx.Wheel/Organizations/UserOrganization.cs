using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Organizations;

public class UserOrganization : AuditedEntity<int>
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
}