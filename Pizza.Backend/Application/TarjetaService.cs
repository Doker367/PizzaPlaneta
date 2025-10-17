
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Application
{
    public class TarjetaService : ITarjetaService
    {
        private readonly ITarjetaRepository _tarjetaRepository;

        public TarjetaService(ITarjetaRepository tarjetaRepository)
        {
            _tarjetaRepository = tarjetaRepository;
        }

        public async Task<List<TarjetaDto>> GetUserCardsAsync(int userId)
        {
            var tarjetas = await _tarjetaRepository.GetByUserIdAsync(userId);
            return tarjetas.Select(t => new TarjetaDto
            {
                Id = t.Id,
                NombreTarjeta = t.NombreTarjeta,
                NumeroEnmascarado = t.NumeroEnmascarado,
                Marca = t.Marca,
                FechaVencimiento = t.FechaVencimiento
            }).ToList();
        }

        public async Task<TarjetaDto> AddCardAsync(int userId, AddTarjetaDto tarjetaDto)
        {
            var nuevaTarjeta = new Tarjeta
            {
                UsuarioId = userId,
                NombreTarjeta = tarjetaDto.NombreTarjeta,
                NumeroEnmascarado = tarjetaDto.NumeroEnmascarado,
                Marca = tarjetaDto.Marca,
                FechaVencimiento = tarjetaDto.FechaVencimiento,
                TokenPago = tarjetaDto.TokenPago
            };

            await _tarjetaRepository.AddAsync(nuevaTarjeta);
            await _tarjetaRepository.SaveChangesAsync();

            return new TarjetaDto
            {
                Id = nuevaTarjeta.Id,
                NombreTarjeta = nuevaTarjeta.NombreTarjeta,
                NumeroEnmascarado = nuevaTarjeta.NumeroEnmascarado,
                Marca = nuevaTarjeta.Marca,
                FechaVencimiento = nuevaTarjeta.FechaVencimiento
            };
        }

        public async Task DeleteCardAsync(int userId, int tarjetaId)
        {
            var tarjeta = await _tarjetaRepository.GetByIdAsync(tarjetaId);

            if (tarjeta == null || tarjeta.UsuarioId != userId)
            {
                // Lanza una excepción si la tarjeta no se encuentra o no pertenece al usuario.
                // Esto previene que un usuario borre tarjetas de otro.
                throw new UnauthorizedAccessException("Tarjeta no encontrada o no tiene permiso para eliminarla.");
            }

            _tarjetaRepository.Delete(tarjeta);
            await _tarjetaRepository.SaveChangesAsync();
        }
    }
}
