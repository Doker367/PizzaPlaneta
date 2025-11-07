
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;
using Stripe;

namespace Pizza.Backend.Application
{
    public class TarjetaService : ITarjetaService
    {
        private readonly ITarjetaRepository _tarjetaRepository;
        private readonly IUserRepository _userRepository;
        private readonly string _stripeSecretKey;

        public TarjetaService(ITarjetaRepository tarjetaRepository, IUserRepository userRepository, IConfiguration configuration)
        {
            _tarjetaRepository = tarjetaRepository;
            _userRepository = userRepository;
            _stripeSecretKey = configuration["Stripe:SecretKey"];
        }

        public async Task<List<TarjetaDto>> GetUserCardsAsync(int userId)
        {
            var tarjetas = await _tarjetaRepository.GetByUserIdAsync(userId);
            return tarjetas.Select(t => new TarjetaDto
            {
                Id = t.Id,
                NombreTarjeta = t.NombreTarjeta,
                Last4 = t.Last4,
                Marca = t.Marca,
                ExpMonth = t.ExpMonth,
                ExpYear = t.ExpYear
            }).ToList();
        }

        public async Task<TarjetaDto> AddCardAsync(int userId, AddTarjetaDto tarjetaDto)
        {
            StripeConfiguration.ApiKey = _stripeSecretKey;

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            if (string.IsNullOrEmpty(user.StripeCustomerId))
            {
                var customerOptions = new CustomerCreateOptions
                {
                    Email = user.Email,
                    Name = user.Nombre,
                };
                var customerService = new CustomerService();
                var customer = await customerService.CreateAsync(customerOptions);
                user.StripeCustomerId = customer.Id;
                await _userRepository.UpdateUserAsync(user);
            }

            var paymentMethodService = new PaymentMethodService();
            var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions { Token = tarjetaDto.TokenPago },
            });

            await paymentMethodService.AttachAsync(paymentMethod.Id, new PaymentMethodAttachOptions
            {
                Customer = user.StripeCustomerId,
            });

            var nuevaTarjeta = new Tarjeta
            {
                UsuarioId = userId,
                NombreTarjeta = tarjetaDto.NombreTarjeta,
                StripePaymentMethodId = paymentMethod.Id,
                Last4 = paymentMethod.Card.Last4,
                Marca = paymentMethod.Card.Brand,
                ExpMonth = (int)paymentMethod.Card.ExpMonth,
                ExpYear = (int)paymentMethod.Card.ExpYear,
                FechaGuardado = DateTime.UtcNow
            };

            await _tarjetaRepository.AddAsync(nuevaTarjeta);
            await _tarjetaRepository.SaveChangesAsync();

            return new TarjetaDto
            {
                Id = nuevaTarjeta.Id,
                NombreTarjeta = nuevaTarjeta.NombreTarjeta,
                Last4 = nuevaTarjeta.Last4,
                Marca = nuevaTarjeta.Marca,
                ExpMonth = nuevaTarjeta.ExpMonth,
                ExpYear = nuevaTarjeta.ExpYear
            };
        }

        public async Task DeleteCardAsync(int userId, int tarjetaId)
        {
            var tarjeta = await _tarjetaRepository.GetByIdAsync(tarjetaId);

            if (tarjeta == null || tarjeta.UsuarioId != userId)
            {
                throw new UnauthorizedAccessException("Tarjeta no encontrada o no tiene permiso para eliminarla.");
            }

            StripeConfiguration.ApiKey = _stripeSecretKey;

            var service = new PaymentMethodService();
            await service.DetachAsync(tarjeta.StripePaymentMethodId);

            _tarjetaRepository.Delete(tarjeta);
            await _tarjetaRepository.SaveChangesAsync();
        }
    }
}
