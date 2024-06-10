using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Menus;

public interface IMenuManager : ISingletonDependency
{
    Task InitializeAsync();
    Menu? GetMenu(string name);

    Menu CreateMenu(string path, string name, string permission, Meta meta, string? component = null,
        string? redirect = null);

    List<Menu> GetExpandedMenus();
    List<Menu> GetShrunkMenus();
}