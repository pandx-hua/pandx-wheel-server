using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Notifications;

public class NotificationSubscription : AuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    [StringLength(256)] public string NotificationName { get; set; } = default!;
}