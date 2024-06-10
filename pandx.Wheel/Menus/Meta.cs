namespace pandx.Wheel.Menus;

public class Meta
{
    public Meta(string icon, string title, string link, bool isHide, bool isFull, bool isAffix, bool isKeepAlive)
    {
        Icon = icon;
        Title = title;
        Link = link;
        IsHide = isHide;
        IsFull = isFull;
        IsAffix = isAffix;
        IsKeepAlive = isKeepAlive;
    }

    public string Icon { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public bool IsHide { get; set; }
    public bool IsFull { get; set; }
    public bool IsAffix { get; set; }
    public bool IsKeepAlive { get; set; }
}