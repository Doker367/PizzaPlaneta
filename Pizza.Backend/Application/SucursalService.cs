using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Application;

public class SucursalService : ISucursalService
{
    private readonly ISucursalRepository _sucursalRepository;
    private readonly MainDbContext _mainDbContext;
    private readonly IProductRepository _productRepository;

    public SucursalService(ISucursalRepository sucursalRepository, MainDbContext mainDbContext, IProductRepository productRepository)
    {
        _sucursalRepository = sucursalRepository;
        _mainDbContext = mainDbContext;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Sucursale>> GetAllAsync()
    {
        return await _sucursalRepository.GetAllAsync();
    }

    public async Task<IEnumerable<MenuItemDto>> GetMenuBySucursalId(int sucursalId)
    {
        var menuItems = await _mainDbContext.Menus
            .Where(m => m.SucursalId == sucursalId && m.Disponible)
            .ToListAsync();

        if (!menuItems.Any())
        {
            return new List<MenuItemDto>();
        }

        var productIds = menuItems.Select(m => m.ProductoId).ToList();
        var products = await _productRepository.GetProductsByIds(productIds);

        var productDict = products.ToDictionary(p => p.Id);

        var menuDto = menuItems.Select(mi =>
        {
            if (productDict.TryGetValue(mi.ProductoId, out var product))
            {
                return new MenuItemDto
                {
                    ProductoId = product.Id,
                    Nombre = product.Nombre,
                    Descripcion = product.Descripcion,
                    Precio = mi.PrecioEspecial ?? product.Precio,
                    Calorias = product.Calorias,
                    Disponible = mi.Disponible
                };
            }
            return null;
        }).Where(m => m != null);

        return menuDto;
    }
}