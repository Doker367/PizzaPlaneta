using ManyBoxApi.DTOs;

namespace ManyBoxApi.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetClientesAsync(string? nombre, string? correo, int skip = 0, int take = 12);
        Task<ClienteDto?> GetClienteByIdAsync(int id);
        Task<ClienteDto> CreateClienteAsync(CrearClienteRequest request);
        Task<bool> DeleteClienteAsync(int id);
    }
}
