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
    private readonly IMenuRepository _menuRepository;

    public SucursalService(ISucursalRepository sucursalRepository, MainDbContext mainDbContext, IProductRepository productRepository, IMenuRepository menuRepository)
    {
        _sucursalRepository = sucursalRepository;
        _mainDbContext = mainDbContext;
        _productRepository = productRepository;
        _menuRepository = menuRepository;
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

        // Filtra la categoría "Extras" (temporalmente deshabilitado para depuración)
        var filteredProducts = products; // products.Where(p => p.Categoria != "Extras");

        var productDict = filteredProducts.ToDictionary(p => p.Id);

        var menuDto = menuItems.Select(mi =>
        {
            if (productDict.TryGetValue(mi.ProductoId, out var product))
            {
                return new MenuItemDto
                {
                    ProductoId = product.Id,
                    Nombre = product.Nombre,
                    Descripcion = product.Descripcion,
                    Categoria = product.Categoria,
                    Precio = mi.PrecioEspecial ?? product.Precio,
                    Calorias = product.Calorias,
                    Disponible = mi.Disponible
                };
            }
            return null;
        }).Where(m => m != null);

        return menuDto;
    }

    public async Task<Sucursale> CreateAsync(CreateSucursalDto sucursalDto)
    {
        var newSucursal = new Sucursale
        {
            Nombre = sucursalDto.Nombre,
            Direccion = sucursalDto.Direccion,
            Ciudad = sucursalDto.Ciudad,
            Estado = sucursalDto.Estado,
            Telefono = sucursalDto.Telefono,
            GoogleMapsUrl = sucursalDto.GoogleMapsUrl
        };

        return await _sucursalRepository.AddAsync(newSucursal);
    }

    public async Task<Menu> AddMenuItemAsync(int sucursalId, AddMenuItemDto menuItemDto)
    {
        var newMenuItem = new Menu
        {
            SucursalId = sucursalId,
            ProductoId = menuItemDto.ProductoId,
            PrecioEspecial = menuItemDto.PrecioEspecial,
            Disponible = true
        };

        return await _menuRepository.AddAsync(newMenuItem);
    }
}