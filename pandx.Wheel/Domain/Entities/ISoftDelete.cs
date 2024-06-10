namespace pandx.Wheel.Domain.Entities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}