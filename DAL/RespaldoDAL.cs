using System;
using System.Data.SqlClient;
using System.IO;

namespace DAL
{
    public class RespaldoDAL
    {
        private const string DatabaseName = "Maneja360";
        private readonly DAO _dao = new DAO();

        public string RealizarBackup(string backupPath)
        {
            var nombreArchivo = $"{DatabaseName}.BCK_{DateTime.Now:ddMMyy_HHmm}.bak";
            var rutaCompleta = Path.Combine(backupPath, nombreArchivo);
            var comandoBackup = $"BACKUP DATABASE {DatabaseName} TO DISK='{rutaCompleta}'";

            _dao.ExecuteNonQuery(comandoBackup);
            return rutaCompleta;
        }

        public void RealizarRestore(string backupFilePath)
        {
            using (var conn = DAO.CreateConnection())
            {
                conn.Open();

                using (var setMaster = new SqlCommand("USE master;", conn))
                {
                    setMaster.ExecuteNonQuery();
                }

                using (var setSingleUser = new SqlCommand($"ALTER DATABASE {DatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", conn))
                {
                    setSingleUser.ExecuteNonQuery();
                }

                var query = $"RESTORE DATABASE {DatabaseName} FROM DISK = '{backupFilePath}' WITH REPLACE;";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var setMultiUser = new SqlCommand($"ALTER DATABASE {DatabaseName} SET MULTI_USER;", conn))
                {
                    setMultiUser.ExecuteNonQuery();
                }
            }
        }
    }
}