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
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<Producto?> GetByIdAsync(int productId)
    {
        return await _context.Productos.FindAsync(productId);
    }

    public async Task<IEnumerable<Producto>> GetAllAsync()
    {
        return await _context.Productos.ToListAsync();
    }

    public async Task<List<Producto>> GetProductsByIds(List<int> productIds)
    {
        return await _context.Productos
                             .Where(p => productIds.Contains(p.Id))
                             .ToListAsync();
    }

    public async Task<Producto> AddAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return producto;
    }

    public async Task UpdateAsync(Producto producto)
    {
        _context.Entry(producto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int productId)
    {
        var producto = await _context.Productos.FindAsync(productId);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}
