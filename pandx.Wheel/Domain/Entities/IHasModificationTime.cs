namespace pandx.Wheel.Domain.Entities;

public interface IHasModificationTime
{
    DateTime? ModificationTime { get; set; }
}