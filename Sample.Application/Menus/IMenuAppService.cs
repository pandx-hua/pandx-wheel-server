using pandx.Wheel.Application.Services;
using pandx.Wheel.Models;
using Sample.Application.Menus.Dto;

namespace Sample.Application.Menus;

public interface IMenuAppService : IApplicationService
{
    Task<ListResponse<MenuDto>> GetMenusAsync();
}