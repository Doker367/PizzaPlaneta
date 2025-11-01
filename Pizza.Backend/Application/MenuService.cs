using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Application;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<IEnumerable<MenuDto>> GetAllAsync()
    {
        var menus = await _menuRepository.GetAllAsync();
        return menus.Select(menu => new MenuDto
        {
            Id = menu.Id,
            SucursalId = menu.SucursalId,
            ProductoId = menu.ProductoId,
            PrecioEspecial = menu.PrecioEspecial,
            Disponible = menu.Disponible
        });
    }
}
