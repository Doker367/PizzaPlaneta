using ManyBoxApi.DTOs;
using ManyBoxApi.Models;
using ManyBoxApi.Repositories;

namespace ManyBoxApi.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<ClienteDto>> GetClientesAsync(string? nombre, string? correo, int skip = 0, int take = 12)
        {
            var clientes = await _clienteRepository.GetClientesAsync(nombre, correo, skip, take);
            return clientes.Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Correo = c.Correo,
                Telefono = c.Telefono
            });
        }

        public async Task<ClienteDto?> GetClienteByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetClienteByIdAsync(id);
            if (cliente == null) return null;

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Correo = cliente.Correo,
                Telefono = cliente.Telefono
            };
        }

        public async Task<ClienteDto> CreateClienteAsync(CrearClienteRequest request)
        {
            var cliente = new Cliente
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Correo = request.Correo,
                Telefono = request.Telefono
            };

            var nuevoCliente = await _clienteRepository.CrearClienteAsync(cliente);

            return new ClienteDto
            {
                Id = nuevoCliente.Id,
                Nombre = nuevoCliente.Nombre,
                Apellido = nuevoCliente.Apellido,
                Correo = nuevoCliente.Correo,
                Telefono = nuevoCliente.Telefono
            };
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            return await _clienteRepository.DeleteClienteAsync(id);
        }
    }
}
