using System;

namespace ManyBoxApi.DTOs
{
    public class NotificacionDto
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public string Prioridad { get; set; }
    }
}
