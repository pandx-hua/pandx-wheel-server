namespace pandx.Wheel.Domain.Entities;

public interface IAudited : IHasCreationTime, IHasModificationTime, IHasDeletionTime
{
    Guid Creator { get; set; }
    Guid? Deleter { get; set; }
    Guid? Modifier { get; set; }
}