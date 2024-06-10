namespace pandx.Wheel.Menus;

public class MenuContext : IMenuContext
{
    public MenuContext(IMenuManager manager)
    {
        Manager = manager;
    }

    public IMenuManager Manager { get; }
}