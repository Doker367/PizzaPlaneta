using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Infrastructure.Repositories;

public class SucursalRepository : ISucursalRepository
{
    private readonly MainDbContext _context;

    public SucursalRepository(MainDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sucursale>> GetAllAsync()
    {
        return await _context.Sucursales.ToListAsync();
    }

    public async Task<Sucursale> AddAsync(Sucursale sucursal)
    {
        _context.Sucursales.Add(sucursal);
        await _context.SaveChangesAsync();
        return sucursal;
    }
}
