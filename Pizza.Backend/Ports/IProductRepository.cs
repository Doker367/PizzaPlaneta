using Pizza.Backend.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Ports;

public interface IProductRepository
{
    Task<Producto?> GetByIdAsync(int productId);
    Task<IEnumerable<Producto>> GetAllAsync();
    Task<List<Producto>> GetProductsByIds(List<int> productIds);
    Task<Producto> AddAsync(Producto producto);
    Task UpdateAsync(Producto producto);
    Task DeleteAsync(int productId);
}
