
using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports;

public interface IMenuRepository
{
    Task<IEnumerable<Menu>> GetAllAsync();
    Task<Menu?> GetByIdAsync(int menuId);
    Task<Menu> AddAsync(Menu menu);
    Task UpdateAsync(Menu menu);
    Task DeleteAsync(int menuId);
}
