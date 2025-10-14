using ManyBoxApi.Data;
using ManyBoxApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ManyBoxApi.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> GetClientesAsync(string? nombre, string? correo, int skip = 0, int take = 12)
        {
            var query = _context.Clientes.AsNoTracking();
            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(c => c.Nombre.Contains(nombre));
            if (!string.IsNullOrEmpty(correo))
                query = query.Where(c => c.Correo.Contains(correo));
            return await query.OrderBy(c => c.Nombre).ThenBy(c => c.Apellido).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<Cliente?> GetClienteByIdAsync(int id)
        {
            return await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente> CrearClienteAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return false;
            }
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
