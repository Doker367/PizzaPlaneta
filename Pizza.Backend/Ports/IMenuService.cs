using Pizza.Backend.Domain;
using Pizza.Backend.Application.DTOs;

namespace Pizza.Backend.Ports;

public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetAllAsync();
    Task<Menu?> GetByIdAsync(int id);
    Task<Menu> UpdateAsync(int id, UpdateMenuItemDto menuItemDto);
    Task DeleteAsync(int id);
}
