using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE;
using DAL;

namespace BLL
{
    public class DVBL
    {
        private readonly DVDAL _dal = new DVDAL();

        private static readonly string[] Tablas =
        {
            "BitacoraEvento",
            "Idioma",
            "Perfil",
            "PerfilJerarquia",
            "Usuario",
            "UsuarioPerfil"
        };

        public void RecalcularDV()
        {
            foreach (var tabla in Tablas)
            {
                RecalcularDV(tabla);
            }
        }

        public void RecalcularDV(string tabla)
        {
            var dt = _dal.ObtenerTabla(tabla);
            var dv = GenerarDVTabla(dt);
            _dal.Guardar(dv);
        }

        public (bool, IDictionary<string, IList<string>>) VerificarDV()
        {
            var errorsGrouped = new Dictionary<string, IList<string>>();
            
            var storedDvs = _dal.ObtenerTodos().ToArray();
            
            if (storedDvs.Length == 0)
            {
                errorsGrouped["DV"] = new List<string> { "No hay dígitos de verificación de datos (DV) almacenados." };
                return (false, errorsGrouped);
            }
            
            bool result = true;

            foreach (var stored in storedDvs)
            {
                var dt = _dal.ObtenerTabla(stored.TableName);
                
                var current = GenerarDVTabla(dt);
                
                if (current == stored) continue;

                // Si no es igual vamos a recorrer los hashes para identificar los campos que están mal.

                result = false;

                if (!errorsGrouped.TryGetValue(stored.TableName, out var errors))
                {
                    errors = new List<string>();
                    errorsGrouped[stored.TableName] = errors;
                }

                if (current.Hash.Length != stored.Hash.Length)
                {
                    errors.Add($"Cantidad de filas diferente: esperado {stored.Hash.Length}, encontrado {current.Hash.Length}");
                    continue;
                }

                if (current.Hash[0].Length != stored.Hash[0].Length)
                {
                    errors.Add($"Cantidad de columnas diferente: esperado {stored.Hash[0].Length}, encontrado {current.Hash[0].Length}");
                    continue;
                }

                for (var i = 0; i < current.Hash.Length; i++)
                {
                    for (var j = 0; j < current.Hash[i].Length; j++)
                    {
                        if (current.Hash[i][j] != stored.Hash[i][j])
                        {
                            // Agregamos el error indicando la fila y nombre de columna donde se encontró la discrepancia.
                            errors.Add($"Fila {i + 1}, Columna {dt.Columns[j].ColumnName}: Valor esperado {stored.Hash[i][j]}, encontrado {current.Hash[i][j]}");
                        }
                    }
                }
            }
            return (result, errorsGrouped);
        }

        private static DV GenerarDVTabla(DataTable dt)
        {
            var hash = GenerarHashTabla(dt);

            return new DV(dt.TableName, hash);
        }

        private static long[][] GenerarHashTabla(DataTable dt)
        {
            var hash = new long[dt.Rows.Count][];

            for(var i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                hash[i] = new long[row.ItemArray.Length];

                for (var j = 0; j < row.ItemArray.Length; j++)
                {
                    var item = row.ItemArray[j];
                    var dv = GenerarDV(item.ToString());
                    hash[i][j] = dv;
                }
            }

            return hash;
        }

        private static long GenerarDV(string texto)
        {
            long sum = 0;
            var bytes = Encoding.UTF8.GetBytes(texto);

            for (var i = 0; i < bytes.Length; i++)
            {
                sum += bytes[i] * (i + 1);
            }

            return sum;
        }
    }
}