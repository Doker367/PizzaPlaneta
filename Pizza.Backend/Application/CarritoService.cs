
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
                return new CarritoDto(); // Devuelve un carrito vacío si no existe
            }
            return MapCarritoToDto(carrito);
        }

        public async Task<CarritoDto> AddItemToCartAsync(int userId, AddItemToCartDto itemDto)
        {
            var producto = await _productRepository.GetByIdAsync(itemDto.ProductoId);
            if (producto == null)
            {
                throw new System.Exception("Producto no encontrado"); // Considerar una excepción más específica
            }

            var carrito = await _carritoRepository.GetByUserIdAsync(userId);
            if (carrito == null)
            {
                carrito = await _carritoRepository.CreateAsync(new Carrito { UsuarioId = userId });
                await _carritoRepository.SaveChangesAsync(); // Guardar para obtener el ID del carrito
                carrito = await _carritoRepository.GetByUserIdAsync(userId); // Re-obtener con includes
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
            return MapCarritoToDto(carritoActualizado);
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
            return MapCarritoToDto(carritoActualizado);
        }

        private CarritoDto MapCarritoToDto(Carrito? carrito)
        {
            if (carrito == null)
            {
                return new CarritoDto();
            }

            return new CarritoDto
            {
                Items = carrito.Items.Select(item => new CarritoItemDto
                {
                    ProductoId = item.ProductoId,
                    NombreProducto = item.Producto?.Nombre ?? string.Empty,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Producto?.Precio ?? 0
                }).ToList()
            };
        }
    }
}
