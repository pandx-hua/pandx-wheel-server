using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Menus;

public interface IMenuProvider : ITransientDependency
{
    Task SetMenusAsync(IMenuContext context);
}