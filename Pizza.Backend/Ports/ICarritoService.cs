
using System.Threading.Tasks;
using Pizza.Backend.Application.DTOs;

namespace Pizza.Backend.Ports
{
    public interface ICarritoService
    {
        Task<CarritoDto> GetCartByUserIdAsync(int userId);
        Task<CarritoDto> AddItemToCartAsync(int userId, AddItemToCartDto itemDto);
        Task<CarritoDto> RemoveItemFromCartAsync(int userId, int productoId);
        Task<CarritoDto> UpdateItemQuantityAsync(int userId, int productoId, int cantidad);
    }
}
