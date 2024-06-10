using pandx.Wheel.Menus;
using pandx.Wheel.Models;
using Sample.Application.Menus.Dto;

namespace Sample.Application.Menus;

public class MenuAppService : SampleAppServiceBase, IMenuAppService
{
    private readonly IMenuManager _menuManager;

    public MenuAppService(IMenuManager menuManager)
    {
        _menuManager = menuManager;
    }

    public async Task<ListResponse<MenuDto>> GetMenusAsync()
    {
        //此处通过Mapper实现深拷贝，避免原始数据被修改
        var menus = Mapper.Map<List<Menu>>(_menuManager.GetShrunkMenus());
        for (var i = menus.Count - 1; i >= 0; i--)
        {
            await RemoveMenuRecursively(menus[i], menus);
        }

        return new ListResponse<MenuDto>(Mapper.Map<List<MenuDto>>(menus));
    }

    private async Task RemoveMenuRecursively(Menu menu, List<Menu> menus)
    {
        if (!await UserService.HasPermissionAsync(CurrentUser.GetUserId(), menu.Permission) ||
            (menu.Component is null && menu.Children.Count == 0))
        {
            menus.Remove(menu);
        }

        menu.Redirect = menu.Children.FirstOrDefault()?.Path;
        for (var i = menu.Children.Count - 1; i >= 0; i--)
        {
            await RemoveMenuRecursively(menu.Children[i], menu.Children);
        }
    }
}