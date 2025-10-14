using ManyBoxApi.Data;
using ManyBoxApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ManyBoxApi.Repositories
{
    public class EnvioRepository : IEnvioRepository
    {
        private readonly AppDbContext _context;

        public EnvioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Envio>> GetEnviosAsync()
        {
            return await _context.Envios
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Envio?> GetEnvioByIdAsync(int id)
        {
            return await _context.Envios
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Envio> CrearEnvioAsync(Envio envio)
        {
            _context.Envios.Add(envio);
            await _context.SaveChangesAsync();
            return envio;
        }
    }
}
