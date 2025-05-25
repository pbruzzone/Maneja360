using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DAO
    {
        public string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["Maneja360"].ConnectionString;

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public DataTable ExecuteDataset(string cmdText, object param = null)
        {
            return ExecuteDataset(cmd => SetupCommand(cmd, cmdText, param));
        }

        private DataTable ExecuteDataset(Action<SqlCommand> cmdAction)
        {
            var dt = new DataTable();
            if (cmdAction == null) return dt;

            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmdAction(cmd);
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public void ExecuteNonQuery(string cmdText, object param = null)
        {
            ExecuteNonQuery(cmd => SetupCommand(cmd, cmdText, param));
        }

        private void ExecuteNonQuery(Action<SqlCommand> cmdAction)
        {
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmdAction(cmd);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string cmdText, object param = null)
        {
            return ExecuteScalar(cmd => SetupCommand(cmd, cmdText, param));
        }

        private object ExecuteScalar(Action<SqlCommand> cmdAction)
        {
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmdAction(cmd);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result;
                }
            }
        }

        private static void SetupCommand(SqlCommand cmd, string cmdText, object param)
        {
            cmd.CommandText = cmdText;

            if (param == null) return;

            var props = param.GetType().GetProperties();

            foreach (var pi in props)
            {
                if (!pi.CanRead) continue;
                cmd.Parameters.AddWithValue(pi.Name, pi.GetValue(param) ?? DBNull.Value);
            }
        }
    }
}