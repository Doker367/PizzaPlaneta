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
}
