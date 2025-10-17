using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizza.Backend.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly PizzaPlanetaContext _context;

    public ProductRepository(PizzaPlanetaContext context)
    {
        _context = context;
    }

    public async Task<Producto?> GetByIdAsync(int productId)
    {
        return await _context.Productos.FindAsync(productId);
    }

    public async Task<List<Producto>> GetProductsByIds(List<int> productIds)
    {
        return await _context.Productos
                             .Where(p => productIds.Contains(p.Id))
                             .ToListAsync();
    }
}
