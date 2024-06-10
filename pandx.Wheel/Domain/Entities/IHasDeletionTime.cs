namespace pandx.Wheel.Domain.Entities;

public interface IHasDeletionTime
{
    DateTime? DeletionTime { get; set; }
}