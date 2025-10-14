using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    [Table("sucursales")]
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        [Column("sucursaldireccion")]
        [JsonPropertyName("direccion")] // el API expone/acepta "direccion" para compatibilidad con el cliente
        public string? SucursalDireccion { get; set; }
    }
}