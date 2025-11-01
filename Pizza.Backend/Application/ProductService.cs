using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Application;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Producto>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Producto?> GetByIdAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Producto> CreateAsync(CreateProductDto productDto)
    {
        var producto = new Producto
        {
            Nombre = productDto.Nombre,
            Descripcion = productDto.Descripcion,
            Precio = productDto.Precio,
            Categoria = productDto.Categoria,
            Calorias = productDto.Calorias,
            Activo = productDto.Activo ?? true
        };
        return await _productRepository.AddAsync(producto);
    }

    public async Task<Producto> UpdateAsync(int id, UpdateProductDto productDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");
        }

        existingProduct.Nombre = productDto.Nombre ?? existingProduct.Nombre;
        existingProduct.Descripcion = productDto.Descripcion ?? existingProduct.Descripcion;
        existingProduct.Precio = productDto.Precio ?? existingProduct.Precio;
        existingProduct.Categoria = productDto.Categoria ?? existingProduct.Categoria;
        existingProduct.Calorias = productDto.Calorias ?? existingProduct.Calorias;
        existingProduct.Activo = productDto.Activo ?? existingProduct.Activo;

        await _productRepository.UpdateAsync(existingProduct);
        return existingProduct;
    }

    public async Task DeleteAsync(int id)
    {
        await _productRepository.DeleteAsync(id);
    }
}
