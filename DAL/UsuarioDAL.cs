using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace DAL
{
    public class UsuarioDAL 
    {
        private readonly DAO _dao = new DAO();
        private readonly PerfilDAL _perfilDAL = new PerfilDAL();

        public void Eliminar(Usuario entity)
        {
            const string query = "Update Usuario SET Activo = 0 WHERE UsuarioId = @UsuarioId";

            _dao.ExecuteNonQuery(query, new { entity.UsuarioId });
        }

        public IEnumerable<Usuario> Listar()
        {
            var result = _dao.Query<Usuario>(
                cmdText:GetSelectStatement(), 
                callbackFn: usr => _perfilDAL.FillUserComponents(usr));
            
            return result;
        }

        public Usuario ObtenerPorNombreDeUsuario(string nombreUsuario)
        {
            if (string.IsNullOrEmpty(nombreUsuario)) return null;
            
            var cmdText = GetSelectStatement() + " WHERE u.NombreUsuario = @NombreUsuario";
            
            var result = _dao.QuerySingleOrDefault<Usuario>(
                cmdText, 
                param: new { nombreUsuario }, 
                callbackFn: usr => _perfilDAL.FillUserComponents(usr));
            
            return result;
        }

        private static string GetSelectStatement()
        {
            return @"SELECT u.*, 
                            i.IdiomaId AS Idioma_IdiomaId, 
                            i.Cultura AS Idioma_Cultura, 
                            i.Descripcion As Idioma_Descripcion
                     FROM Usuario u 
                     INNER JOIN Idioma i on u.IdiomaId = i.IdiomaId";
        }

        public Usuario Obtener(int id)
        {
            throw new NotImplementedException();
        }

        public void Guardar(Usuario usr)
        {
            if (usr == null) return;

            using (var scope = new TransactionScope())
            {
                using (var cnn = new SqlConnection(DAO.ConnectionString))
                {
                    cnn.Open();
                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@Nombre", usr.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", usr.Apellido);
                        cmd.Parameters.AddWithValue("@DNI", usr.DNI);
                        cmd.Parameters.AddWithValue("@Mail", usr.Mail);
                        cmd.Parameters.AddWithValue("@NombreUsuario", usr.NombreUsuario);
                        cmd.Parameters.AddWithValue("@Bloqueado", usr.Bloqueado);
                        cmd.Parameters.AddWithValue("@Activo", usr.Activo);
                        cmd.Parameters.AddWithValue("@IdiomaId", usr.Idioma.IdiomaId);
                        cmd.Parameters.AddWithValue("@ResetPassword", usr.ResetPassword);

                        if (usr.UsuarioId == 0)
                        {
                            cmd.CommandText = @"INSERT INTO Usuario OUTPUT INSERTED.UsuarioId VALUES
                                                (@Nombre, @Apellido, @DNI, @Mail, @NombreUsuario, 
                                                @Password, @Bloqueado, @Activo, @IdiomaId, @ResetPassword)";
                            cmd.Parameters.AddWithValue("@Password", usr.Password);
                            usr.UsuarioId = (int)cmd.ExecuteScalar();
                        }
                        else
                        {
                            cmd.CommandText = @"Update Usuario
                                                SET Nombre = @Nombre,
                                                    Apellido = @Apellido,
                                                    DNI = @DNI,
                                                    Mail = @Mail,
                                                    NombreUsuario = @NombreUsuario,
                                                    Bloqueado = @Bloqueado,
                                                    Activo = @Activo,
                                                    IdiomaId = @IdiomaId,
                                                    ResetPassword = @ResetPassword
                                                WHERE UsuarioId = @Id";
                            cmd.Parameters.AddWithValue("@Id", usr.UsuarioId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                GuardarPermisos(usr);

                scope.Complete();
            }
        }

        private void GuardarPermisos(Usuario usr)
        {
            if (usr.Perfiles.Count == 0) return;

            using (var scope = new TransactionScope())
            {
                using (var cnn = new SqlConnection(DAO.ConnectionString))
                {
                    cnn.Open();
                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM UsuarioPerfil WHERE UsuarioId = @UsuarioId;";
                        cmd.Parameters.AddWithValue("@UsuarioId", usr.UsuarioId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO UsuarioPerfil (UsuarioId, PerfilId) 
                                            VALUES (@UsuarioId, @PerfilId)";

                        cmd.Parameters.AddWithValue("@UsuarioId", usr.UsuarioId);
                        cmd.Parameters.Add("@PerfilId", SqlDbType.Int);

                        foreach (var perfil in usr.Perfiles)
                        {
                            cmd.Parameters["@PerfilId"].Value = perfil.PerfilId;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                scope.Complete();
            }
        }

        public void ActualizarPassword(Usuario entity)
        {
            if (entity == null) return;

            const string cmdText = @"Update Usuario
                                     SET Password = @Password,
                                         ResetPassword = @ResetPassword
                                     WHERE UsuarioId = @UsuarioId";

            _dao.ExecuteNonQuery(cmdText,
                new { entity.UsuarioId, entity.Password, entity.ResetPassword });
        }

        public void ActualizarIdioma(Usuario usuario)
        {
            if (usuario == null) return;

            const string cmdText = @"Update Usuario
                                     SET IdiomaId = @IdiomaId
                                     WHERE UsuarioId = @UsuarioId";

            _dao.ExecuteNonQuery(cmdText,
                new { usuario.UsuarioId, usuario.Idioma.IdiomaId });
        }
    }
}
