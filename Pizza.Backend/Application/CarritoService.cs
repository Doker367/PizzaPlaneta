using System.Linq;
using System.Threading.Tasks;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Application
{
    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IProductRepository _productRepository;

        public CarritoService(ICarritoRepository carritoRepository, IProductRepository productRepository)
        {
            _carritoRepository = carritoRepository;
            _productRepository = productRepository;
        }

        public async Task<CarritoDto> GetCartByUserIdAsync(int userId)
        {
            var carrito = await _carritoRepository.GetByUserIdAsync(userId);
            if (carrito == null)
            {
                // Si no hay carrito, creamos uno vacío para asegurar que la app no falle.
                var newCart = await _carritoRepository.CreateAsync(new Carrito { UsuarioId = userId });
                await _carritoRepository.SaveChangesAsync();
                return await MapCarritoToDtoAsync(newCart);
            }
            return await MapCarritoToDtoAsync(carrito);
        }

        public async Task<CarritoDto> AddItemToCartAsync(int userId, AddItemToCartDto itemDto)
        {
            var producto = await _productRepository.GetByIdAsync(itemDto.ProductoId);
            if (producto == null)
            {
                throw new System.Exception("Producto no encontrado");
            }

            var carrito = await _carritoRepository.GetByUserIdAsync(userId);
            if (carrito == null)
            {
                carrito = await _carritoRepository.CreateAsync(new Carrito { UsuarioId = userId });
                await _carritoRepository.SaveChangesAsync();
                carrito = await _carritoRepository.GetByUserIdAsync(userId);
            }

            if (carrito == null)
            {
                throw new System.Exception("No se pudo crear o recuperar el carrito.");
            }

            var itemEnCarrito = await _carritoRepository.GetItemAsync(carrito.Id, itemDto.ProductoId);

            if (itemEnCarrito != null)
            {
                itemEnCarrito.Cantidad += itemDto.Cantidad;
                _carritoRepository.UpdateItem(itemEnCarrito);
            }
            else
            {
                var nuevoItem = new CarritoItem
                {
                    CarritoId = carrito.Id,
                    ProductoId = itemDto.ProductoId,
                    Cantidad = itemDto.Cantidad
                };
                await _carritoRepository.AddItemAsync(nuevoItem);
            }

            await _carritoRepository.SaveChangesAsync();

            var carritoActualizado = await _carritoRepository.GetByUserIdAsync(userId);
            return await MapCarritoToDtoAsync(carritoActualizado);
        }

        public async Task<CarritoDto> RemoveItemFromCartAsync(int userId, int productoId)
        {
            var carrito = await _carritoRepository.GetByUserIdAsync(userId);
            if (carrito == null)
            {
                throw new System.Exception("Carrito no encontrado");
            }

            var itemParaEliminar = await _carritoRepository.GetItemAsync(carrito.Id, productoId);
            if (itemParaEliminar != null)
            {
                _carritoRepository.RemoveItem(itemParaEliminar);
                await _carritoRepository.SaveChangesAsync();
            }

            var carritoActualizado = await _carritoRepository.GetByUserIdAsync(userId);
            return await MapCarritoToDtoAsync(carritoActualizado);
        }

        public async Task<CarritoDto> UpdateItemQuantityAsync(int userId, int productoId, int cantidad)
        {
            var carrito = await _carritoRepository.GetByUserIdAsync(userId);
            if (carrito == null)
            {
                throw new System.Exception("Carrito no encontrado");
            }

            var itemParaActualizar = await _carritoRepository.GetItemAsync(carrito.Id, productoId);
            if (itemParaActualizar != null)
            {
                if (cantidad > 0)
                {
                    itemParaActualizar.Cantidad = cantidad;
                    _carritoRepository.UpdateItem(itemParaActualizar);
                }
                else
                {
                    _carritoRepository.RemoveItem(itemParaActualizar);
                }
                await _carritoRepository.SaveChangesAsync();
            }
            // Si el item no existe, no hacemos nada. Podríamos añadir un item nuevo si quisiéramos.

            var carritoActualizado = await _carritoRepository.GetByUserIdAsync(userId);
            return await MapCarritoToDtoAsync(carritoActualizado);
        }

        private async Task<CarritoDto> MapCarritoToDtoAsync(Carrito? carrito)
        {
            if (carrito == null)
            {
                return new CarritoDto { Items = new System.Collections.Generic.List<CarritoItemDto>() };
            }

            if (carrito.Items == null || !carrito.Items.Any())
            {
                return new CarritoDto { Items = new System.Collections.Generic.List<CarritoItemDto>() };
            }

            var productIds = carrito.Items.Select(i => i.ProductoId).Distinct().ToList();
            var productos = await _productRepository.GetProductsByIds(productIds);
            var prodDict = productos.ToDictionary(p => p.Id);

            return new CarritoDto
            {
                Items = carrito.Items.Select(item =>
                {
                    prodDict.TryGetValue(item.ProductoId, out var prod);
                    return new CarritoItemDto
                    {
                        ProductoId = item.ProductoId,
                        NombreProducto = prod?.Nombre ?? "Producto no encontrado",
                        Cantidad = item.Cantidad,
                        PrecioUnitario = prod?.Precio ?? 0
                    };
                }).ToList()
            };
        }
    }
}