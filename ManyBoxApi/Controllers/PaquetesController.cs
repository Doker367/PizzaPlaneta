using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using ManyBoxApi.DTOs;
using ManyBoxApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization; // Agregado
using System.Security.Claims; // Agregado
using QDocument = QuestPDF.Fluent.Document; // Evitar ambigüedad con ManyBoxApi.Models.Document

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Proteger todo el controlador
    public class PaquetesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<PaquetesController> _logger;

        public PaquetesController(AppDbContext context, IWebHostEnvironment env, ILogger<PaquetesController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // --- Private Helpers ---
        private int? GetEmpleadoIdFromClaims()
        {
            var empleadoIdClaim = User.FindFirst("EmpleadoId");
            if (empleadoIdClaim != null && int.TryParse(empleadoIdClaim.Value, out int empleadoId))
            {
                return empleadoId;
            }
            return null;
        }

        private int? GetSucursalIdFromClaims()
        {
            var sucursalIdClaim = User.FindFirst("SucursalId");
            if (sucursalIdClaim != null && int.TryParse(sucursalIdClaim.Value, out int sucursalId))
            {
                return sucursalId;
            }
            return null;
        }

        // GET: api/paquetes
        // Este endpoint ahora filtra de forma segura segun el rol del usuario.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaqueteDto>>> GetPaquetes()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var empleadoId = GetEmpleadoIdFromClaims();
            var sucursalId = GetSucursalIdFromClaims();

            IQueryable<Venta> ventasQuery = _context.Ventas;

            if (userRole == "Admin" && sucursalId.HasValue)
            {
                var empleadosEnSucursal = _context.Empleados.Where(e => e.SucursalId == sucursalId.Value).Select(e => e.Id);
                ventasQuery = ventasQuery.Where(v => v.Empleado_Id.HasValue && empleadosEnSucursal.Contains(v.Empleado_Id.Value));
            }
            else if (userRole != "SuperAdmin" && empleadoId.HasValue)
            {
                ventasQuery = ventasQuery.Where(v => v.Empleado_Id == empleadoId.Value);
            }
            // SuperAdmin no tiene filtros

            var ventas = await ventasQuery
                .Include(v => v.Remitente)
                .Include(v => v.Destinatario)
                .Include(v => v.DetalleContenido)
                .Include(v => v.Envios)
                    .ThenInclude(e => e.Seguimientos)
                .Include(v => v.Envios)
                    .ThenInclude(e => e.Empleado) // Incluir el empleado (chofer) del envio
                .ToListAsync();

            // Eficiencia: Cargar todos los empleados una sola vez.
            var todosLosEmpleados = await _context.Empleados.Include(e => e.Sucursal).ToListAsync();

            var paquetes = ventas.Select(v => MapVentaToPaqueteDto(v, todosLosEmpleados)).ToList();
            return Ok(paquetes);
        }

        // Alias para compatibilidad con el cliente MAUI: /api/paquetes/filtrados
        [HttpGet("filtrados")]
        public Task<ActionResult<IEnumerable<PaqueteDto>>> GetPaquetesFiltrados([FromQuery] int? usuarioId, [FromQuery] string? rol)
        {
            // Ignora query params y aplica los filtros por Claims.
            return GetPaquetes();
        }

        // GET: api/paquetes/seguimiento/{guia}
        [HttpGet("seguimiento/{guia}")]
        [AllowAnonymous] // Permitir consulta publica de seguimiento
        public async Task<ActionResult<SeguimientoPaqueteDto>> ObtenerSeguimientoPorGuia(string guia)
        {
            var envio = await _context.Envios
                .AsNoTracking()
                .Include(e => e.Venta)
                    .ThenInclude(v => v.Destinatario)
                .Include(e => e.Empleado)
                    .ThenInclude(emp => emp.Sucursal)
                .Include(e => e.Seguimientos)
                .FirstOrDefaultAsync(e => e.GuiaRastreo == guia);

            if (envio == null)
                return NotFound();

            var historial = envio.Seguimientos
                .OrderBy(s => s.FechaStatus)
                .Select(s => new MovimientoSeguimientoDto
                {
                    Fecha = s.FechaStatus,
                    Descripcion = s.Status
                }).ToList();

            string sucursalOrigen = envio.Empleado?.Sucursal?.Nombre ?? "";
            string ciudadDestino = envio.Venta?.Destinatario?.Ciudad ?? "";

            var ultimoSeguimiento = envio.Seguimientos.OrderByDescending(s => s.FechaStatus).FirstOrDefault();

            var dto = new SeguimientoPaqueteDto
            {
                Id = envio.GuiaRastreo,
                Estado = ultimoSeguimiento?.Status ?? "Desconocido",
                FechaActualizacion = ultimoSeguimiento?.FechaStatus ?? DateTime.MinValue,
                Origen = sucursalOrigen,
                Destino = ciudadDestino,
                Historial = historial
            };
            return Ok(dto);
        }

        // GET: api/paquetes/guia-por-venta/{ventaId}
        [HttpGet("guia-por-venta/{ventaId}")]
        public async Task<ActionResult<object>> GetGuiaPorVentaId(int ventaId)
        {
            var envio = await _context.Envios.FirstOrDefaultAsync(e => e.VentaId == ventaId);
            if (envio == null)
                return NotFound();
            return Ok(new { guiaRastreo = envio.GuiaRastreo });
        }

        // GET: api/paquetes/{id}/nota-venta-pdf
        [HttpGet("{id}/nota-venta-pdf")]
        public async Task<IActionResult> DescargarNotaVentaPDF(int id)
        {
            // Asegurar licencia de QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            try
            {
                var venta = await _context.Ventas
                    .Include(v => v.Remitente)
                    .Include(v => v.Destinatario)
                    .Include(v => v.DetalleContenido)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venta == null) return NotFound("Venta no encontrada.");

                var folio = venta.Folio ?? $"VENTA-{venta.Id}";

                string sucursal = string.Empty;
                if (venta.Empleado_Id.HasValue)
                {
                    var emp = await _context.Empleados
                        .Include(e => e.Sucursal)
                        .FirstOrDefaultAsync(e => e.Id == venta.Empleado_Id.Value);
                    sucursal = emp?.Sucursal?.Nombre ?? string.Empty;
                }

                var fecha = venta.Fecha;
                var remitenteNombre = venta.Remitente?.Nombre ?? string.Empty;
                var remitenteTelefono = venta.Remitente?.Telefono ?? string.Empty;
                var destinatarioNombre = venta.Destinatario?.Nombre ?? string.Empty;
                var destinatarioTelefono = venta.Destinatario?.Telefono ?? string.Empty;
                var direccionDestino = venta.Destinatario?.Direccion ?? string.Empty;
                var ciudadDestino = venta.Destinatario?.Ciudad ?? string.Empty;
                var tipoEnvio = venta.Compania_Envio ?? string.Empty;
                var totalPiezas = venta.Total_Piezas ?? 0;
                var peso = venta.Peso_Fisico ?? 0m;
                var costo = venta.Costo_Envio ?? 0m;

                var terminos = new[]
                {
                    "1.- EN ESTA EMPRESA NO SOMOS RESPONSABLES DE LAS REVISIONES ADUANALES GUBERNAMENTALES EN EL PAÍS DE ORIGEN COMO EN EL PAÍS DE DESTINO.",
                    "2.- LAS REVISIONES, DESECHOS Y/O RETORNOS DE PRODUCTOS ESTÁN SUJETOS EN TODO MOMENTO POR AGENTES ADUANALES GUBERNAMENTALES DEL PAÍS DE ORIGEN COMO DEL PAÍS DE DESTINO.",
                    "3.- LOS ENVÍOS INTERNACIONALES LLEGAN A SU DESTINO DE 3 A 7 DÍAS HÁBILES DESPUÉS DE HACER SU ENVÍO SALVO CUALQUIER REVISIÓN ADUANAL GUBERNAMENTAL, DEMORA DE TRÁNSITO, CAMBIOS CLIMÁTICOS EXTREMOS Y/O DESASTRES NATURALES LO CUAL PUEDE DEMORAR LA ENTREGA.",
                    "4.- LOS ENVÍOS NACIONALES ESTÁN SUJETOS AL SERVICIO DE GUÍA QUE CONTRATE YA SEA EXPRESS DE 1 A 3 DÍAS HÁBILES Y ESTÁNDAR DE 3 A 7 DÍAS HÁBILES SI NO EXISTE DEMORA ADUANAL GUBERNAMENTAL.",
                    "5.- ESTA EMPRESA ES INTERMEDIARIA POR CONVENIO CON LAS EMPRESAS DHL Y FEDEX, POR LO TANTO PACK FLASH NO TRASLADA MERCANCÍA A DESTINO, SOLO ES UN PUNTO DE RECEPCIÓN.",
                    "6.- LOS CAMBIOS DE DOMICILIO ESTÁN SUJETOS A REVISIÓN LO CUAL PUEDE GENERAR UN COSTO EXTRA.",
                    "7.- ESTÁ PROHIBIDO ENVIAR PRODUCTOS OCULTOS DENTRO DE MERCANCÍA DECLARADA, SI EL PAQUETE ES RETORNADO POR CONTENER PRODUCTO OCULTO NO HABRÁ DEVOLUCIÓN DE DINERO Y SERÁ ACREEDOR A UNA MULTA EQUIVALENTE AL PESO ENVIADO.",
                    "8.- LA EMPRESA PACK FLASH NO SE HACE RESPONSABLE EN ENVÍOS QUE VIAJAN BAJO RESPONSABILIDAD DEL CLIENTE POR LO TANTO CUALQUIER RETARDO, DESECHO Y/O RETORNO NO HABRÁ DEVOLUCIONES DE DINERO.",
                    "9.- LOS NÚMEROS TELEFÓNICOS PARA DUDAS O ACLARACIONES ESTÁN AL PIE DE CADA LOGO DE LA COMPAÑÍA POR LA QUE SE ENVÍA EL PAQUETE.",
                    "10.- CUALQUIER ACLARACIÓN FAVOR DE CONTACTARSE AL TELÉFONO DE ATENCIÓN INDICADO EN ESTE DOCUMENTO."
                };

                var telefonoAtencion = "TELEFONO DE ATENCION: 9613595384";

                var pdf = QDocument.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(25);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"NOTA DE VENTA").FontSize(18).SemiBold().FontColor(Colors.Black);
                                row.ConstantItem(180).AlignRight().Column(c =>
                                {
                                    c.Item().AlignRight().Text($"Folio: {folio}").SemiBold();
                                    c.Item().AlignRight().Text($"Fecha: {fecha:dd/MM/yyyy HH:mm}");
                                    c.Item().AlignRight().Text($"Sucursal: {sucursal}");
                                });
                            });

                            col.Item().PaddingTop(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        });

                        page.Content().Column(col =>
                        {
                            col.Item().Background(Colors.Grey.Lighten4).Padding(8).Text("Datos de la operación").SemiBold();
                            col.Item().PaddingVertical(6).Grid(grid =>
                            {
                                grid.Columns(2);
                                grid.Item().Column(c =>
                                {
                                    c.Item().Text(t => { t.Span("Remitente: ").SemiBold(); t.Span(remitenteNombre); });
                                    c.Item().Text(t => { t.Span("Tel. Remitente: ").SemiBold(); t.Span(remitenteTelefono); });
                                    c.Item().Text(t => { t.Span("Tipo de envío: ").SemiBold(); t.Span(tipoEnvio); });
                                    c.Item().Text(t => { t.Span("Piezas: ").SemiBold(); t.Span(totalPiezas.ToString()); });
                                });
                                grid.Item().Column(c =>
                                {
                                    c.Item().Text(t => { t.Span("Destinatario: ").SemiBold(); t.Span(destinatarioNombre); });
                                    c.Item().Text(t => { t.Span("Tel. Destinatario: ").SemiBold(); t.Span(destinatarioTelefono); });
                                    c.Item().Text(t => { t.Span("Ciudad destino: ").SemiBold(); t.Span(ciudadDestino); });
                                    c.Item().Text(t => { t.Span("Dirección destino: ").SemiBold(); t.Span(direccionDestino); });
                                });
                            });

                            col.Item().PaddingTop(6).Row(r =>
                            {
                                r.RelativeItem().Text(t => { t.Span("Peso: ").SemiBold(); t.Span($"{peso:0.###} kg"); });
                                r.RelativeItem().Text(t => { t.Span("Costo de envío: ").SemiBold(); t.Span($"${costo:0.00}"); });
                            });

                            col.Item().PaddingTop(10).Row(r =>
                            {
                                r.RelativeItem().Text(telefonoAtencion).FontSize(12).SemiBold().FontColor(Colors.Orange.Darken1);
                            });

                            col.Item().PaddingTop(6).AlignCenter().Text("TERMINOS Y CONDICIONES").FontSize(14).SemiBold().FontColor(Colors.Red.Medium);

                            col.Item().PaddingTop(6).Grid(grid =>
                            {
                                grid.Columns(2);

                                grid.Item().PaddingRight(8).Column(c =>
                                {
                                    c.Item().Text("NOMBRE Y FIRMA DEL REMITENTE").SemiBold().FontSize(12);
                                    c.Item().PaddingTop(6).Height(60).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                    c.Item().PaddingTop(8).Text("ACEPTO QUE LOS DATOS PROPORCIONADOS EN EL PRESENTE DOCUMENTO HAN SIDO REVISADOS POR MÍ Y ESTOY DE ACUERDO QUE SON CORRECTOS").FontSize(9);
                                    c.Item().Text("LOS DÍAS CONSIDERADOS SON HÁBILES").FontSize(9);
                                    c.Item().Text("ACEPTO LAS CONDICIONES Y RIESGOS DEL ENVÍO").FontSize(9);
                                });

                                // Columna derecha: Lista de términos (tamaño más pequeño)
                                grid.Item().PaddingLeft(8).Column(c =>
                                {
                                    for (int i = 0; i < terminos.Length; i++)
                                    {
                                        var t = terminos[i];
                                        c.Item().Text(t).FontSize(8).LineHeight(1.1f);
                                    }
                                });
                            });
                        });

                        page.Footer()
                            .DefaultTextStyle(x => x.FontColor(Colors.Grey.Darken2))
                            .Row(row =>
                            {
                                row.RelativeItem().AlignLeft().Text($"Generado por ManyBox");
                                row.RelativeItem().AlignRight().Text(t =>
                                {
                                    t.Span("Página ");
                                    t.CurrentPageNumber();
                                    t.Span(" / ");
                                    t.TotalPages();
                                });
                            });
                    });
                }).GeneratePdf();

                var safeFile = string.IsNullOrWhiteSpace(folio) ? $"NotaVenta_{id}.pdf" : $"NotaVenta_{folio}.pdf";
                return File(pdf, "application/pdf", safeFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando PDF para venta {VentaId}", id);

                var fallback = QDocument.Create(c =>
                {
                    c.Page(p =>
                    {
                        p.Size(PageSizes.A4);
                        p.Margin(25);
                        p.Content().Column(col =>
                        {
                            col.Item().Text("Nota de Venta").FontSize(18).SemiBold();
                            col.Item().Text($"No fue posible generar el documento detallado. Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                    });
                }).GeneratePdf();
                return File(fallback, "application/pdf", $"NotaVenta_{id}.pdf");
            }
        }

        private PaqueteDto MapVentaToPaqueteDto(Venta v, List<Empleado> empleados)
        {
            var envio = v.Envios?.OrderByDescending(e => e.Id).FirstOrDefault();
            string estadoActual = "Registrado";
            DateTime? fechaUltima = v.Fecha;
            string repartidorNombre = string.Empty;
            string repartidorTelefono = string.Empty;
            string numeroGuia = string.Empty;

            if (envio != null)
            {
                var chofer = envio.EmpleadoId != null ? empleados.FirstOrDefault(e => e.Id == envio.EmpleadoId) : null;
                if (chofer != null)
                {
                    repartidorNombre = chofer.Nombre;
                    repartidorTelefono = chofer.Telefono;
                }
                if (envio.Seguimientos != null && envio.Seguimientos.Any())
                {
                    var ultimoSeguimiento = envio.Seguimientos.OrderByDescending(s => s.FechaStatus).FirstOrDefault();
                    if (ultimoSeguimiento != null)
                    {
                        estadoActual = ultimoSeguimiento.Status;
                        fechaUltima = ultimoSeguimiento.FechaStatus;
                    }
                }
                numeroGuia = envio.GuiaRastreo ?? string.Empty;
            }

            var empleadoRegistro = v.Empleado_Id != null ? empleados.FirstOrDefault(e => e.Id == v.Empleado_Id) : null;

            return new PaqueteDto
            {
                Id = v.Id,
                VentaId = v.Id,
                CodigoSeguimiento = v.Folio ?? $"VENTA-{v.Id}",
                FechaRegistro = v.Fecha,
                SucursalOrigen = empleadoRegistro?.Sucursal?.Nombre,
                RemitenteNombre = v.Remitente?.Nombre ?? string.Empty,
                RemitenteTelefono = v.Remitente?.Telefono ?? string.Empty,
                DestinatarioNombre = v.Destinatario?.Nombre ?? string.Empty,
                DestinatarioTelefono = v.Destinatario?.Telefono ?? string.Empty,
                DireccionDestino = v.Destinatario?.Direccion ?? string.Empty,
                CiudadDestino = v.Destinatario?.Ciudad ?? string.Empty,
                TipoEnvio = v.Compania_Envio ?? string.Empty,
                EstadoActual = estadoActual,
                FechaUltimaActualizacion = fechaUltima,
                EmpleadoRegistro = empleadoRegistro?.Nombre,
                EmpleadoActual = empleadoRegistro?.Nombre ?? string.Empty,
                RepartidorAsignado = !string.IsNullOrEmpty(repartidorNombre) ? $"{repartidorNombre} ({repartidorTelefono})" : string.Empty,
                PesoKg = v.Peso_Fisico ?? 0,
                NumeroGuia = !string.IsNullOrEmpty(numeroGuia) ? numeroGuia : (v.Folio ?? $"VENTA-{v.Id}"),
                PaquetesAsignados = v.Total_Piezas?.ToString() ?? "-",
                CostoEnvio = v.Costo_Envio ?? 0
            };
        }
    }
}