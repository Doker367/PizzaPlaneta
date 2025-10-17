
using System.Collections.Generic;
using System.Threading.Tasks;
using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports
{
    public interface ITarjetaRepository
    {
        Task<Tarjeta?> GetByIdAsync(int tarjetaId);
        Task<List<Tarjeta>> GetByUserIdAsync(int userId);
        Task AddAsync(Tarjeta tarjeta);
        void Delete(Tarjeta tarjeta);
        Task<int> SaveChangesAsync();
    }
}
