using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DAL
{
    public class BitacoraEventoDAL
    {
        private readonly DAO _dao = new DAO();

        public IList<BitacoraEvento> Listar()
        {
            const string query = @"SELECT BitacoraEventoId, 
                                          NombreUsuario, 
                                          Fecha, 
                                          Hora, 
                                          Modulo,
                                          Evento,
                                          Criticidad
                                    FROM BitacoraEvento";

            var dt = _dao.ExecuteDataset(query);

            var list = Map(dt);

            return list;
        }

        public IList<BitacoraEvento> Listar(string nombreUsuario, DateTime? fechaIni, DateTime? fechaFin, Modulo modulo, Evento evento, Criticidad criticidad)
        {
            const string query = @"SELECT BitacoraEventoId, 
                                          NombreUsuario, 
                                          Fecha, 
                                          Hora, 
                                          Modulo,
                                          Evento,
                                          Criticidad
                                   FROM BitacoraEvento
                                   WHERE (@NombreUsuario IS NULL OR NombreUsuario = @NombreUsuario)
                                   AND ((@FechaIni IS NULL OR @FechaFin IS NULL) OR (Fecha >= @FechaIni AND Fecha <= @FechaFin))  
                                   AND (@Modulo IS NULL OR Modulo = @Modulo)
                                   AND (@Evento IS NULL OR Evento = @Evento)
                                   AND (@Criticidad IS NULL OR Criticidad = @Criticidad)";

            var dt = _dao.ExecuteDataset(query,
                new
                {
                    NombreUsuario = string.IsNullOrWhiteSpace(nombreUsuario) ? null : nombreUsuario,
                    FechaIni = fechaIni,
                    FechaFin = fechaFin,
                    Modulo = modulo == Modulo.Ninguno ? (object)null : modulo.Codigo,
                    Evento = evento == Evento.Ninguno ? (object)null : evento.Codigo,
                    Criticidad = criticidad == Criticidad.Ninguna ? (object)null : criticidad
                });

            var list = Map(dt);

            return list;
        }

        private static List<BitacoraEvento> Map(DataTable dt)
        {
            var query = from row in dt.AsEnumerable()
                        select new BitacoraEvento
                        {
                            BitacoraEventoId = row.Field<int>("BitacoraEventoId"),
                            NombreUsuario = row.Field<string>("NombreUsuario"),
                            Fecha = row.Field<DateTime>("Fecha"),
                            Hora = row.Field<TimeSpan>("Hora"),
                            Modulo = Modulo.ObtenerPorCodigo(row.Field<int>("Modulo")),
                            Evento = Evento.ObtenerPorCodigo(row.Field<int>("Evento")),
                            Criticidad = row.Field<Criticidad>("Criticidad")
                        };

            var list = query.ToList();
            return list;
        }

        public void Guardar(BitacoraEvento entity)
        {
            if (entity == null) return;

            const string cmdText = @"INSERT INTO BitacoraEvento (
                                         NombreUsuario, 
                                         Fecha, 
                                         Hora, 
                                         Modulo,
                                         Evento,
                                         Criticidad)  
                                     VALUES (
                                         @NombreUsuario, 
                                         @Fecha, 
                                         @Hora, 
                                         @Modulo,
                                         @Evento,
                                         @Criticidad)";

            _dao.ExecuteNonQuery(cmdText,
                new
                {
                    entity.NombreUsuario,
                    entity.Fecha,
                    entity.Hora,
                    Modulo = entity.Modulo.Codigo,
                    Evento = entity.Evento.Codigo,
                    entity.Criticidad
                });
        }
    }
}
