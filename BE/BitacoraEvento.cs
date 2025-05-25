using System;

namespace BE
{
    public class BitacoraEvento
    {
        public int BitacoraEventoId { get; set; }
        public Criticidad Criticidad { get; set; }
        public Evento Evento { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public Modulo Modulo { get; set; }
        public string NombreUsuario { get; set; }
    }
}