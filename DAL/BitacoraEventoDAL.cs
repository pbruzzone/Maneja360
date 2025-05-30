﻿using BE;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class BitacoraEventoDAL
    {
        private readonly DAO _dao = new DAO();

        public IEnumerable<BitacoraEvento> Listar(string nombreUsuario, DateTime? fechaIni, DateTime? fechaFin, Modulo modulo, Evento evento, Criticidad criticidad)
        {
            const string query = @"SELECT BitacoraEventoId, 
                                          NombreUsuario, 
                                          Fecha, 
                                          Hora, 
                                          Modulo,
                                          Evento,
                                          Criticidad
                                   FROM BitacoraEvento
                                   WHERE (@NombreUsuario IS NULL OR NombreUsuario LIKE '%' + @NombreUsuario + '%')
                                   AND ((@FechaIni IS NULL OR @FechaFin IS NULL) OR (Fecha >= @FechaIni AND Fecha <= @FechaFin))  
                                   AND (@Modulo IS NULL OR Modulo = @Modulo)
                                   AND (@Evento IS NULL OR Evento = @Evento)
                                   AND (@Criticidad IS NULL OR Criticidad = @Criticidad)
                                   ORDER BY Fecha DESC, Hora DESC";


            var result = _dao.Query(
                query,
                MapFn,
                new
                {
                    NombreUsuario = string.IsNullOrWhiteSpace(nombreUsuario) ? null : nombreUsuario,
                    FechaIni = fechaIni,
                    FechaFin = fechaFin,
                    Modulo = modulo == Modulo.Ninguno ? (object)null : modulo.Codigo,
                    Evento = evento == Evento.Ninguno ? (object)null : evento.Codigo,
                    Criticidad = criticidad == Criticidad.Ninguna ? (object)null : criticidad
                }
            );

            return result;
        }

        private static BitacoraEvento MapFn(IDataReader r)
        {
            return new BitacoraEvento
            {
                BitacoraEventoId = (int)r["BitacoraEventoId"],
                NombreUsuario = (string)r["NombreUsuario"],
                Fecha = (DateTime)r["Fecha"],
                Hora = (TimeSpan)r["Hora"],
                Modulo = Modulo.ObtenerPorCodigo((int)r["Modulo"]),
                Evento = Evento.ObtenerPorCodigo((int)r["Evento"]),
                Criticidad = (Criticidad)r["Criticidad"]
            };
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
