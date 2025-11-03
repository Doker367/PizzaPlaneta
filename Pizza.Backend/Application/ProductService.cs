using Microsoft.Extensions.Caching.Memory;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Application;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMemoryCache _cache;
    private const string ProductsCacheKey = "AllProducts";

    public ProductService(IProductRepository productRepository, IMemoryCache cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<IEnumerable<Producto>> GetAllAsync()
    {
        if (!_cache.TryGetValue(ProductsCacheKey, out IEnumerable<Producto> products))
        {
            var productList = (await _productRepository.GetAllAsync()).ToList();

            // --- VISUAL CHANGE FOR FRONTEND ---
            if (productList.Any())
            {
                productList.First().Nombre = "[OFERTA] " + productList.First().Nombre;
            }
            // ------------------------------------

            products = productList;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
            
            _cache.Set(ProductsCacheKey, products, cacheEntryOptions);
        }
        return products;
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
        var newProduct = await _productRepository.AddAsync(producto);
        _cache.Remove(ProductsCacheKey); // Invalidate cache
        return newProduct;
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
        _cache.Remove(ProductsCacheKey); // Invalidate cache
        return existingProduct;
    }

    public async Task DeleteAsync(int id)
    {
        await _productRepository.DeleteAsync(id);
        _cache.Remove(ProductsCacheKey); // Invalidate cache
    }
}
