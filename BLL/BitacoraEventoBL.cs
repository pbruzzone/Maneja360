using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace BLL
{
    public class BitacoraEventoBL
    {
        private readonly BitacoraEventoDAL _dal = new BitacoraEventoDAL();

        public void Guardar(Evento evento, Modulo modulo, Criticidad criticidad, string nombreUsuario)
        {
            var fechaActual = DateTime.Now;
            var evt = new BitacoraEvento
            {
                NombreUsuario = nombreUsuario,
                Fecha = fechaActual.Date,
                Hora = fechaActual.TimeOfDay,
                Evento = evento,
                Modulo = modulo,
                Criticidad = criticidad
            };

            Guardar(evt);
        }

        public void Guardar(BitacoraEvento entity)
        {
            _dal.Guardar(entity);
        }

        public IEnumerable<BitacoraEvento> Listar(
            string nombreUsuario, 
            DateTime? fechaIni, 
            DateTime? fechaFin, 
            Modulo modulo, 
            Evento evento, 
            Criticidad criticidad)
        {
            return _dal.Listar(nombreUsuario, fechaIni, fechaFin, modulo, evento, criticidad);
        }
    }
}
