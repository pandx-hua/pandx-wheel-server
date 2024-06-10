namespace pandx.Wheel.Domain.Entities;

public interface IEntity
{
}

public interface IEntity<TPrimaryKey> : IEntity
{
    TPrimaryKey Id { get; set; }
}