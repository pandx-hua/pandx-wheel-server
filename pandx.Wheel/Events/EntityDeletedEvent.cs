using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Events;

public class EntityDeletedEvent<TEntity> : Event where TEntity : IEntity
{
    public EntityDeletedEvent(TEntity entity)
    {
        Entity = entity;
    }

    public TEntity Entity { get; private set; }
}