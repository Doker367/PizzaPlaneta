using SQLite;


namespace ManyBox.Models.Locals
{
    [Table("Remitentes")]
    public class RemitenteLocal
    {
        [PrimaryKey]
        [Column("IdRemitenteAsociado")]
        public Guid IdRemitenteAsociado { get; set; }

        [Column("IdRemitente")]
        public Guid IdRemitente { get; set; }

        [Column("Nombre")]
        public string? Nombre { get; set; }

        [Column("Direccion")]
        public string? Direccion { get; set; }

        [Column("Ciudad")]
        public string? Ciudad { get; set; }

        [Column("Estado")]
        public string? Estado { get; set; }

        [Column("Pais")]
        public string? Pais { get; set; } 

        [Column("CP")]
        public string? CP { get; set; }

        [Column("Celular")]
        public string? Celular { get; set; }
    }
}
