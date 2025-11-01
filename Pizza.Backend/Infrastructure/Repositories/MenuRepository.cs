using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Infrastructure.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly MainDbContext _context;

    public MenuRepository(MainDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Menu>> GetAllAsync()
    {
        return await _context.Menus.ToListAsync();
    }

    public async Task<Menu> AddAsync(Menu menu)
    {
        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    public async Task<Menu?> GetByIdAsync(int menuId)
    {
        return await _context.Menus.FindAsync(menuId);
    }

    public async Task UpdateAsync(Menu menu)
    {
        _context.Entry(menu).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int menuId)
    {
        var menu = await _context.Menus.FindAsync(menuId);
        if (menu != null)
        {
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
        }
    }
}
