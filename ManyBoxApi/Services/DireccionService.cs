using ManyBoxApi.DTOs;
using ManyBoxApi.Models;
using ManyBoxApi.Repositories;

namespace ManyBoxApi.Services
{
    public class DireccionService : IDireccionService
    {
        private readonly IDireccionRepository _direccionRepository;

        public DireccionService(IDireccionRepository direccionRepository)
        {
            _direccionRepository = direccionRepository;
        }

        public async Task<IEnumerable<DireccionDto>> GetDireccionesAsync()
        {
            var direcciones = await _direccionRepository.GetDireccionesAsync();
            return direcciones.Select(d => new DireccionDto
            {
                Id = d.Id,
                ClienteId = d.ClienteId,
                DireccionTexto = d.DireccionTexto,
                Ciudad = d.Ciudad,
                Estado = d.Estado,
                Pais = d.Pais,
                CodigoPostal = d.CodigoPostal
            });
        }

        public async Task<DireccionDto> CreateDireccionAsync(CrearDireccionRequest request)
        {
            var direccion = new Direccion
            {
                ClienteId = request.ClienteId,
                DireccionTexto = request.DireccionTexto,
                Ciudad = request.Ciudad,
                Estado = request.Estado,
                Pais = request.Pais,
                CodigoPostal = request.CodigoPostal
            };

            var nuevaDireccion = await _direccionRepository.CrearDireccionAsync(direccion);

            return new DireccionDto
            {
                Id = nuevaDireccion.Id,
                ClienteId = nuevaDireccion.ClienteId,
                DireccionTexto = nuevaDireccion.DireccionTexto,
                Ciudad = nuevaDireccion.Ciudad,
                Estado = nuevaDireccion.Estado,
                Pais = nuevaDireccion.Pais,
                CodigoPostal = nuevaDireccion.CodigoPostal
            };
        }
    }
}
