using Pizza.Backend.Domain;
using Pizza.Backend.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Ports;

public interface IProductService
{
    Task<IEnumerable<Producto>> GetAllAsync();
    Task<Producto?> GetByIdAsync(int id);
    Task<Producto> CreateAsync(CreateProductDto productDto);
    Task<Producto> UpdateAsync(int id, UpdateProductDto productDto);
    Task DeleteAsync(int id);
}
