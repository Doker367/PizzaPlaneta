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

    public async Task<Menu?> GetByIdAsync(int id)
    {
        return await _menuRepository.GetByIdAsync(id);
    }

    public async Task<Menu> UpdateAsync(int id, UpdateMenuItemDto menuItemDto)
    {
        var existingMenuItem = await _menuRepository.GetByIdAsync(id);
        if (existingMenuItem == null)
        {
            throw new KeyNotFoundException($"Item de menú con ID {id} no encontrado.");
        }

        existingMenuItem.PrecioEspecial = menuItemDto.PrecioEspecial ?? existingMenuItem.PrecioEspecial;
        existingMenuItem.Disponible = menuItemDto.Disponible ?? existingMenuItem.Disponible;

        await _menuRepository.UpdateAsync(existingMenuItem);
        return existingMenuItem;
    }

    public async Task DeleteAsync(int id)
    {
        await _menuRepository.DeleteAsync(id);
    }
}
