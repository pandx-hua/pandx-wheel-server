namespace pandx.Wheel.Application.Dto;

public class EntityDto<TPrimaryKey> : IEntityDto<TPrimaryKey>
{
    public TPrimaryKey Id { get; set; } = default!;
}

public class EntityDto : EntityDto<int>
{
}