namespace Sample.Application.Menus.Dto;

public class MenuDto
{
    public string Path { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Component { get; set; }
    public string? Redirect { get; set; }
    public MetaDto Meta { get; set; } = default!;
    public List<MenuDto>? Children { get; set; } = default!;
}