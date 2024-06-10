using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Events;

public class EntityModifiedEvent<TEntity> : Event where TEntity : IEntity
{
    public EntityModifiedEvent(TEntity entity)
    {
        Entity = entity;
    }

    public TEntity Entity { get; private set; }
}