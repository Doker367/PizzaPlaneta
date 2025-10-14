using ManyBoxApi.DTOs;

namespace ManyBoxApi.Services
{
    public interface IDireccionService
    {
        Task<IEnumerable<DireccionDto>> GetDireccionesAsync();
        Task<DireccionDto> CreateDireccionAsync(CrearDireccionRequest request);
    }
}
