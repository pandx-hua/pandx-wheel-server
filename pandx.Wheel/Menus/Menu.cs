namespace pandx.Wheel.Menus;

public class Menu
{
    public Menu()
    {
    }

    public Menu(string path, string name, string permission, Meta meta, string? component = null,
        string? redirect = null)
    {
        Parent = null;
        Path = path;
        Name = name;
        Permission = permission;
        Meta = meta;
        Component = component;
        Redirect = redirect;
        Children = new List<Menu>();
    }

    public Menu? Parent { get; set; }
    public string Path { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Component { get; set; }
    public string? Redirect { get; set; }
    public string Permission { get; set; } = default!;
    public Meta Meta { get; set; } = default!;
    public List<Menu> Children { get; } = default!;

    public Menu CreateChildMenu(string path, string name, string permission, Meta meta, string? component = null,
        string? redirect = null)
    {
        var menu = new Menu(path, name, permission, meta, component, redirect) { Parent = this };
        Children.Add(menu);
        return menu;
    }

    public override string ToString()
    {
        return $"[Menu:{Name}]";
    }
}