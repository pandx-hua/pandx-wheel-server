namespace pandx.Wheel.Domain.Entities;

public abstract class AuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, IAudited
{
    public DateTime CreationTime { get; set; }
    public Guid Creator { get; set; }
    public DateTime? ModificationTime { get; set; }
    public Guid? Modifier { get; set; }
    public DateTime? DeletionTime { get; set; }
    public Guid? Deleter { get; set; }
}

public abstract class AuditedEntity : AuditedEntity<int>
{
}