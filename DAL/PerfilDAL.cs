using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using BE;
using BE.Composite;

namespace DAL
{
    public class PerfilDAL
    {
        private readonly DAO _dao = new DAO();

        public void Eliminar(Perfil entity)
        {
            const string query = "DELETE FROM Perfil WHERE PerfilId = @PerfilId";

            _dao.ExecuteNonQuery(query, new { entity.PerfilId });
        }

        public IEnumerable<Perfil> Listar()
        {
            const string query = "SELECT PerfilId, Nombre, Permiso FROM Perfil";

            var result = _dao.Query(query, MapFn);
            return result;
        }

        public IEnumerable<Perfil> ObtenerPerfiles()
        {
            const string query = @"SELECT
                                     p.PerfilId,
                                     p.Nombre,
                                     p.Permiso
                                   FROM Perfil p
                                   INNER JOIN PerfilJerarquia pj ON p.PerfilId = pj.HijoId
                                   WHERE pj.PadreId IS NULL";

            var result = _dao.Query(query, MapFn);
            return result;
        }

        public IEnumerable<Perfil> ObtenerPerfilesUsuario(string nombreUsuario)
        {
            const string query = @"SELECT
                                     p.PerfilId,
                                     p.Nombre,
                                     p.Permiso
                                   FROM Perfil p
                                   INNER JOIN PerfilJerarquia pj ON p.PerfilId = pj.HijoId
                                   INNER JOIN UsuarioPerfil up ON p.PerfilId = up.PerfilId 
                                   INNER JOIN Usuario u ON u.UsuarioId = up.UsuarioId
                                   WHERE pj.PadreId IS NULL
                                   AND u.NombreUsuario = @NombreUsuario";

            var result = _dao.Query(query, MapFn, new { nombreUsuario });
            return result;
        }

        private static Perfil MapFn(IDataReader r)
        {
           var perfilId = (int)r["PerfilId"];
           var nombre = r["Nombre"].ToString();
           var permiso = r["Permiso"].ToString();
           return Perfil.Crear(perfilId, nombre, permiso);
        }

        public Perfil Obtener(int id)
        {
            throw new NotImplementedException();
        }

        public void Guardar(Perfil perfil)
        {
            Guardar(perfil, false);
        }

        public void GuardarPerfil(Perfil perfil)
        {
            Guardar(perfil, true);
        }

        private void Guardar(Perfil perfil, bool root)
        {
            if (perfil == null) return;

            using (var scope = new TransactionScope())
            {
                using (var cnn = DAO.CreateConnection())
                {
                    cnn.Open();

                    int id;

                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Perfil(Nombre, Permiso) output INSERTED.PerfilId 
                                            VALUES (@Nombre, @Permiso);";

                        cmd.Parameters.AddWithValue("@Nombre", perfil.Nombre);
                        cmd.Parameters.AddWithValue("@Permiso", perfil.Tipo == TipoPerfil.Familia ? DBNull.Value : (object)perfil.Permiso.ToString());

                        id = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (root)
                    {
                        using (var cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO PerfilJerarquia (PadreId, HijoId) VALUES (@PadreId, @HijoId)";

                            cmd.Parameters.AddWithValue("@PadreId", DBNull.Value);
                            cmd.Parameters.AddWithValue("@HijoId", id);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    scope.Complete();
                    perfil.PerfilId = id;
                }
            }
        }

        public void FillUserComponents(Usuario usr)
        {
            using (var cnn = DAO.CreateConnection())
            {
                cnn.Open();
                var cmd = cnn.CreateCommand();

                cmd.CommandText = @"SELECT p.* 
                                    FROM UsuarioPerfil up 
                                    INNER JOIN Perfil p ON up.PerfilId = p.PerfilId 
                                    WHERE up.UsuarioId = @id;";

                cmd.Parameters.AddWithValue("id", usr.UsuarioId);

                using (var reader = cmd.ExecuteReader())
                {
                    usr.Perfiles.Clear();

                    while (reader.Read())
                    {
                        var permisoId = reader.GetInt32(reader.GetOrdinal("PerfilId"));
                        var permisoNombre = reader.GetString(reader.GetOrdinal("Nombre"));

                        var permisoTipo = string.Empty;
                        if (reader["permiso"] != DBNull.Value)
                            permisoTipo = reader.GetString(reader.GetOrdinal("Permiso"));

                        if (!string.IsNullOrEmpty(permisoTipo))
                        {
                            var p = new Patente
                            {
                                PerfilId = permisoId,
                                Nombre = permisoNombre,
                                Permiso = (TipoPermiso)Enum.Parse(typeof(TipoPermiso), permisoTipo)
                            };

                            usr.Perfiles.Add(p);
                        }
                        else
                        {
                            var f = new Familia
                            {
                                PerfilId = permisoId,
                                Nombre = permisoNombre
                            };

                            var items = ObtenerComponentes(permisoId);

                            foreach (var item in items)
                            {
                                f.AgregarHijo(item);
                            }

                            usr.Perfiles.Add(f);
                        }
                    }
                }
            }
        }

        public IList<Perfil> ObtenerComponentes(int familiaId = 0)
        {
            var where = "IS NULL";

            if (familiaId > 0)
            {
                where = $"={familiaId}";
            }

            using (var cnn = DAO.CreateConnection())
            {
                cnn.Open();
                var cmd = cnn.CreateCommand();

                var sql = $@"WITH Recursivo AS (
                            SELECT pj2.PadreId, pj2.HijoId  
                            FROM PerfilJerarquia pj2
                            WHERE pj2.PadreId {where}  
                            UNION ALL 
                            SELECT pj.PadreId, pj.HijoId 
                            FROM PerfilJerarquia pj 
                            INNER JOIN Recursivo r on r.HijoId = pj.PadreId
                        )
                        SELECT r.PadreId,r.HijoId, p.PerfilId, p.Nombre, p.Permiso
                        FROM Recursivo r 
                        INNER JOIN Perfil p on r.HijoId = p.PerfilId";

                cmd.CommandText = sql;

                using (var reader = cmd.ExecuteReader())
                {
                    var lista = new List<Perfil>();

                    while (reader.Read())
                    {
                        var padreId = 0;
                        if (reader["PadreId"] != DBNull.Value)
                        {
                            padreId = reader.GetInt32(reader.GetOrdinal("PadreId"));
                        }

                        var perfilId = reader.GetInt32(reader.GetOrdinal("PerfilId"));
                        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));

                        string permiso = null;
                        if (reader["Permiso"] != DBNull.Value)
                            permiso = reader.GetString(reader.GetOrdinal("Permiso"));


                        var perfil = Perfil.Crear(perfilId, nombre, permiso);

                        var padre = GetComponent(padreId, lista);

                        if (padre == null)
                        {
                            lista.Add(perfil);
                        }
                        else
                        {
                            padre.AgregarHijo(perfil);
                        }

                    }
                    return lista;
                }
            }
        }

        private static Perfil GetComponent(int id, IList<Perfil> lista)
        {
            var perfil = lista?.Where(i => i.PerfilId == id).FirstOrDefault();

            if (perfil == null && lista != null)
            {
                foreach (var c in lista)
                {
                    var comp = GetComponent(id, c.Hijos);
                    if (comp != null && comp.PerfilId == id) return comp;
                    if (comp != null)
                    {
                        return GetComponent(id, comp.Hijos);
                    }
                }
            }

            return perfil;
        }

        public void GuardarComponentes(Perfil p)
        {
            using (var scope = new TransactionScope())
            {
                using (var cnn = new SqlConnection(DAO.ConnectionString))
                {
                    cnn.Open();
                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.Connection = cnn;
                        cmd.CommandText = "DELETE FROM PerfilJerarquia WHERE PadreId = @PadreId;";
                        cmd.Parameters.AddWithValue("@PadreId", p.PerfilId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO PerfilJerarquia (PadreId, HijoId) VALUES (@PadreId, @HijoId)";
                        cmd.Parameters.AddWithValue("@PadreId", p.PerfilId);
                        cmd.Parameters.Add("@HijoId", SqlDbType.Int);

                        foreach (var item in p.Hijos)
                        {
                            cmd.Parameters["@HijoId"].Value = item.PerfilId;
                            cmd.ExecuteNonQuery();
                        }
                    }

                }
                scope.Complete();
            }

        }

        public bool TieneUsuariosAsignados(int perfilId)
        {
            const string query = "SELECT COUNT(*) FROM UsuarioPerfil WHERE PerfilId = @PerfilId";

            var count = _dao.ExecuteScalar<int>(query, new { PerfilId = perfilId });

            return count > 0;
        }
    }
}