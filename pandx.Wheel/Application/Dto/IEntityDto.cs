namespace pandx.Wheel.Application.Dto;

public interface IEntityDto<TPrimaryKey>
{
    TPrimaryKey Id { get; set; }
}

public interface IEntityDto
{
}