
using System.Collections.Generic;
using System.Threading.Tasks;
using Pizza.Backend.Application.DTOs;

namespace Pizza.Backend.Ports
{
    public interface ITarjetaService
    {
        Task<List<TarjetaDto>> GetUserCardsAsync(int userId);
        Task<TarjetaDto> AddCardAsync(int userId, AddTarjetaDto tarjetaDto);
        Task DeleteCardAsync(int userId, int tarjetaId);
    }
}
