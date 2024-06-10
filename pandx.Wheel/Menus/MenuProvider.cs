namespace pandx.Wheel.Menus;

public abstract class MenuProvider : IMenuProvider
{
    public abstract Task SetMenusAsync(IMenuContext context);
}