using pandx.Wheel.Exceptions;
using pandx.Wheel.Extensions;

namespace pandx.Wheel.Menus;

public class MenuManager : IMenuManager
{
    private readonly IMenuProvider _menuProvider;
    private readonly IDictionary<string, Menu> _menus;

    public MenuManager(IMenuProvider menuProvider)
    {
        _menus = new Dictionary<string, Menu>();
        _menuProvider = menuProvider;
    }

    public async Task InitializeAsync()
    {
        var context = new MenuContext(this);
        await _menuProvider.SetMenusAsync(context);
    }

    public Menu? GetMenu(string name)
    {
        return _menus.GetOrDefault(name);
    }

    public Menu CreateMenu(string path, string name, string permission, Meta meta, string? component = null,
        string? redirect = null)
    {
        if (_menus.ContainsKey(name))
        {
            throw new WheelException($"已经存在名称为 {name} 的菜单");
        }

        var menu = new Menu(path, name, permission, meta, component, redirect);
        _menus[name] = menu;
        return menu;
    }

    public List<Menu> GetExpandedMenus()
    {
        var expandedMenus = new Dictionary<string, Menu>();
        foreach (var menu in _menus.Values.ToList())
        {
            AddMenuRecursively(menu, expandedMenus);
        }

        return expandedMenus.Values.ToList();
    }

    public List<Menu> GetShrunkMenus()
    {
        var menus = _menus.Values.ToList();
        return _menus.Values.ToList();
    }

    private void AddMenuRecursively(Menu menu, Dictionary<string, Menu> expandedMenus)
    {
        if (expandedMenus.TryGetValue(menu.Name, out var existingMenu))
        {
            if (existingMenu != menu)
            {
                throw new WheelException($"已经存在名称为 {menu.Name} 的菜单");
            }
        }
        else
        {
            expandedMenus[menu.Name] = menu;
        }

        foreach (var child in menu.Children)
        {
            AddMenuRecursively(child, expandedMenus);
        }
    }
}