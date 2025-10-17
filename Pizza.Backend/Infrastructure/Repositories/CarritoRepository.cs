
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Infrastructure.Repositories
{
    public class CarritoRepository : ICarritoRepository
    {
        private readonly PizzaPlanetaContext _context;

        public CarritoRepository(PizzaPlanetaContext context)
        {
            _context = context;
        }

        public async Task<Carrito?> GetByUserIdAsync(int userId)
        {
            return await _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);
        }

        public async Task<Carrito> CreateAsync(Carrito carrito)
        {
            await _context.Carritos.AddAsync(carrito);
            return carrito;
        }

        public async Task AddItemAsync(CarritoItem item)
        {
            await _context.CarritoItems.AddAsync(item);
        }

        public async Task<CarritoItem?> GetItemAsync(int carritoId, int productoId)
        {
            return await _context.CarritoItems.FirstOrDefaultAsync(ci => ci.CarritoId == carritoId && ci.ProductoId == productoId);
        }

        public void UpdateItem(CarritoItem item)
        {
            _context.CarritoItems.Update(item);
        }

        public void RemoveItem(CarritoItem item)
        {
            _context.CarritoItems.Remove(item);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
