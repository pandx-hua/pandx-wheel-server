using pandx.Wheel.Events;

namespace pandx.Wheel.Domain.Entities;

public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>, IDomainEvent
{
    public TPrimaryKey Id { get; set; } = default!;
}

public abstract class Entity : Entity<int>
{
}