using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Events;

public class EntityAddedEvent<TEntity> : Event where TEntity : IEntity
{
    public EntityAddedEvent(TEntity entity)
    {
        Entity = entity;
    }

    public TEntity Entity { get; private set; }
}