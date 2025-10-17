using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Infrastructure.Repositories;

public class SucursalRepository : ISucursalRepository
{
    private readonly PizzaPlanetaContext _context;

    public SucursalRepository(PizzaPlanetaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sucursale>> GetAllAsync()
    {
        return await _context.Sucursales.ToListAsync();
    }
}
