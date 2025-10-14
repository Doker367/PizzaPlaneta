using ManyBoxApi.Models;

namespace ManyBoxApi.Repositories
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetClientesAsync(string? nombre, string? correo, int skip = 0, int take = 12);
        Task<Cliente?> GetClienteByIdAsync(int id);
        Task<Cliente> CrearClienteAsync(Cliente cliente);
        Task<bool> DeleteClienteAsync(int id);
    }
}
