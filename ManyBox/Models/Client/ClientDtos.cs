namespace ManyBox.Models.Client
{
    // Venta / Registro
    public class CrearVentaRequest
    {
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int? Cliente_Id { get; set; }
        public int? Empleado_Id { get; set; }
        public string Folio { get; set; } = string.Empty;
        public string Remitente_Nombre { get; set; } = string.Empty;
        public string Remitente_Telefono { get; set; } = string.Empty;
        public string? Remitente_Compania { get; set; }
        public string Remitente_Direccion { get; set; } = string.Empty;
        public string Remitente_Ciudad { get; set; } = string.Empty;
        public string Remitente_Estado { get; set; } = string.Empty;
        public string Remitente_Pais { get; set; } = string.Empty;
        public string Remitente_Cp { get; set; } = string.Empty;
        public string Destinatario_Nombre { get; set; } = string.Empty;
        public string Destinatario_Telefono { get; set; } = string.Empty;
        public string? Destinatario_Compania { get; set; }
        public string Destinatario_Direccion { get; set; } = string.Empty;
        public string Destinatario_Ciudad { get; set; } = string.Empty;
        public string Destinatario_Estado { get; set; } = string.Empty;
        public string Destinatario_Pais { get; set; } = string.Empty;
        public string Destinatario_Cp { get; set; } = string.Empty;
        public decimal? Valor_Declarado { get; set; }
        public string Medidas { get; set; } = string.Empty;
        public decimal? Peso_Volumetrico { get; set; }
        public decimal? Peso_Fisico { get; set; }
        public bool? Seguro { get; set; }
        public string Compania_Envio { get; set; } = string.Empty;
        public string Tipo_Riesgo { get; set; } = string.Empty;
        public string Tipo_Pago { get; set; } = string.Empty;
        public decimal? Costo_Envio { get; set; }
        public int? Total_Piezas { get; set; }
        public string Tiempo_Estimado { get; set; } = string.Empty;
        public decimal? Total_Cobrado { get; set; }
        public List<DetalleContenidoRequest> DetalleContenido { get; set; } = new();
        public bool Completada { get; set; }
    }
    public class DetalleContenidoRequest
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public string? Unidad { get; set; }
    }

    // Dashboard / Estadísticas
    public class DashboardStatsDto
    {
        public int EnviosActivos { get; set; }
        public int TasaEntrega { get; set; }
        public int Satisfaccion { get; set; }
        public int NuevosPaquetes { get; set; }
    }
    public class DashboardStatsHistoryDto
    {
        public int EnviosActivos { get; set; }
        public int TasaEntrega { get; set; }
        public int Satisfaccion { get; set; }
        public int NuevosPaquetes { get; set; }
        public DateTime Fecha { get; set; }
    }
    public class EntregasMensualesDto
    {
        public int Mes { get; set; }
        public int Entregados { get; set; }
        public int Pendientes { get; set; }
    }
    public class SucursalRendimientoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Porcentaje { get; set; }
    }
    public class ActividadRecienteDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Icono { get; set; } = string.Empty;
    }

    // Notificaciones
    public class NotificacionDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Prioridad { get; set; } = string.Empty;
    }
    public class CrearNotificacionRequest
    {
        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string? Datos { get; set; }
        public DateTime? Expiracion { get; set; }
        public int UsuarioId { get; set; }
        public int SucursalId { get; set; }
        public int RolId { get; set; }
    }
    public class NotificacionUsuarioVM
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? Datos { get; set; }
        public DateTime? Expiracion { get; set; }
        public bool Leido { get; set; }
    }
    public class ArchivoUploadResponse
    {
        public string? Url { get; set; }
        public string? NombreOriginal { get; set; }
    }

    // Paquetes
    public class PaqueteDto
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public string CodigoSeguimiento { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string? SucursalOrigen { get; set; }
        public string RemitenteNombre { get; set; } = string.Empty;
        public string RemitenteTelefono { get; set; } = string.Empty;
        public string DestinatarioNombre { get; set; } = string.Empty;
        public string DestinatarioTelefono { get; set; } = string.Empty;
        public string DireccionDestino { get; set; } = string.Empty;
        public string CiudadDestino { get; set; } = string.Empty;
        public string TipoEnvio { get; set; } = string.Empty;
        public string EstadoActual { get; set; } = string.Empty;
        public DateTime? FechaUltimaActualizacion { get; set; }
        public string? EmpleadoRegistro { get; set; }
        public string EmpleadoActual { get; set; } = string.Empty;
        public string RepartidorAsignado { get; set; } = string.Empty;
        public decimal PesoKg { get; set; }
        public string NumeroGuia { get; set; } = string.Empty;
        public string PaquetesAsignados { get; set; } = string.Empty;
        public decimal CostoEnvio { get; set; }
    }

    // Rutas / Asignación
    public class AsignarEnvioRequest
    {
        public string CodigoSeguimiento { get; set; } = string.Empty;
        public int EmpleadoId { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public int VentaId { get; set; }
    }

    // Seguimiento
    public class SeguimientoPaqueteDto
    {
        public string Id { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaActualizacion { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public List<MovimientoSeguimientoDto> Historial { get; set; } = new();
    }
    public class MovimientoSeguimientoDto
    {
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    // Bitácora chofer
    public class BitacoraEntregaDto
    {
        public string Guia { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
