using ManyBoxApi.DTOs;
using ManyBoxApi.Models;
using ManyBoxApi.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;

namespace ManyBoxApi.Services
{
    public class EnvioService : IEnvioService
    {
        private readonly IEnvioRepository _envioRepository;
        private readonly AppDbContext _context;

        public EnvioService(IEnvioRepository envioRepository, AppDbContext context)
        {
            _envioRepository = envioRepository;
            _context = context;
        }

        public async Task<IEnumerable<EnvioDto>> GetAllEnviosAsync()
        {
            var envios = await _envioRepository.GetEnviosAsync();
            return envios.Select(e => new EnvioDto
            {
                Id = e.Id,
                VentaId = e.VentaId,
                FechaEntrega = e.FechaEntrega
            });
        }

        public async Task<EnvioDto?> GetEnvioByIdAsync(int id)
        {
            var envio = await _envioRepository.GetEnvioByIdAsync(id);
            if (envio == null) return null;

            return new EnvioDto
            {
                Id = envio.Id,
                VentaId = envio.VentaId,
                FechaEntrega = envio.FechaEntrega
            };
        }

        public async Task<EnvioDto> CreateEnvioAsync(CrearEnvioRequest request)
        {
            var envio = new Envio
            {
                VentaId = request.VentaId,
                FechaEntrega = request.FechaEntrega
            };

            var nuevoEnvio = await _envioRepository.CrearEnvioAsync(envio);

            return new EnvioDto
            {
                Id = nuevoEnvio.Id,
                VentaId = nuevoEnvio.VentaId,
                FechaEntrega = nuevoEnvio.FechaEntrega
            };
        }

        public async Task<IEnumerable<object>> GetEnviosFiltradosAsync(string rol, int? empleadoId, int? sucursalId)
        {
            var enviosQuery = _context.Envios
                .Include(e => e.Venta)
                .Include(e => e.Venta.Remitente)
                .Include(e => e.Venta.Destinatario)
                .Include(e => e.Seguimientos)
                .AsQueryable();

            var empleados = await _context.Empleados.Include(e => e.Sucursal).ToListAsync();

            if (!string.IsNullOrEmpty(rol))
            {
                rol = rol.ToLower();
                if (rol == "empleado" && empleadoId.HasValue)
                {
                    // Solo los envíos registrados por el empleado
                    enviosQuery = enviosQuery.Where(e => e.Venta.Empleado_Id == empleadoId.Value);
                }
                else if (rol == "admin" && sucursalId.HasValue)
                {
                    // Todos los envíos de la sucursal del admin
                    var empleadosSucursal = empleados.Where(emp => emp.SucursalId == sucursalId.Value).Select(emp => emp.Id).ToList();
                    enviosQuery = enviosQuery.Where(e => e.Venta.Empleado_Id != null && empleadosSucursal.Contains(e.Venta.Empleado_Id.Value));
                }
                // superadmin: no filtra
            }

            var envios = await enviosQuery.ToListAsync();

            var result = envios.Select(e => new
            {
                e.Id,
                e.GuiaRastreo,
                e.FechaEntrega,
                Status = e.Seguimientos != null && e.Seguimientos.Count > 0 ? e.Seguimientos.OrderByDescending(s => s.Id).FirstOrDefault()?.Status : null,
                Venta = e.Venta == null ? null : new {
                    e.Venta.Id,
                    e.Venta.Folio,
                    e.Venta.Fecha,
                    e.Venta.Total_Cobrado,
                    Remitente = e.Venta.Remitente == null ? null : new {
                        e.Venta.Remitente.Nombre,
                        e.Venta.Remitente.Telefono
                    },
                    Destinatario = e.Venta.Destinatario == null ? null : new {
                        e.Venta.Destinatario.Nombre,
                        e.Venta.Destinatario.Telefono
                    }
                }
            });

            return result;
        }
    }
}
