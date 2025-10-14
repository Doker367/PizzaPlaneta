using ManyBoxApi.DTOs;
using ManyBoxApi.Models;

namespace ManyBoxApi.Repositories
{
    public interface IEnvioRepository
    {
        Task<IEnumerable<Envio>> GetEnviosAsync();
        Task<Envio?> GetEnvioByIdAsync(int id);
        Task<Envio> CrearEnvioAsync(Envio envio);
    }
}
