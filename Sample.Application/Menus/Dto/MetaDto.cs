namespace Sample.Application.Menus.Dto;

public class MetaDto
{
    public string Icon { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Link { get; set; } = default!;
    public bool IsHide { get; set; }
    public bool IsFull { get; set; }
    public bool IsAffix { get; set; }
    public bool IsKeepAlive { get; set; }
}