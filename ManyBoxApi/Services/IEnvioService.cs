using ManyBoxApi.DTOs;

namespace ManyBoxApi.Services
{
    public interface IEnvioService
    {
        Task<IEnumerable<EnvioDto>> GetAllEnviosAsync();
        Task<EnvioDto?> GetEnvioByIdAsync(int id);
        Task<EnvioDto> CreateEnvioAsync(CrearEnvioRequest request);
        Task<IEnumerable<object>> GetEnviosFiltradosAsync(string rol, int? empleadoId, int? sucursalId);
    }
}
