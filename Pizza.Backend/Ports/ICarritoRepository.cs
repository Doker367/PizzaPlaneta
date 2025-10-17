
using System.Threading.Tasks;
using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports
{
    public interface ICarritoRepository
    {
        Task<Carrito?> GetByUserIdAsync(int userId);
        Task<Carrito> CreateAsync(Carrito carrito);
        Task AddItemAsync(CarritoItem item);
        Task<CarritoItem?> GetItemAsync(int carritoId, int productoId);
        void UpdateItem(CarritoItem item);
        void RemoveItem(CarritoItem item);
        Task SaveChangesAsync();
    }
}
