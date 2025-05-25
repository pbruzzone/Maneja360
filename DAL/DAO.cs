using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DAL
{
    public class DAO
    {
        public const string DatabaseName = "Maneja360";
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;

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

        public static void EnsureDatabaseExists()
        {
            try
            {
                EnsureDatabaseExistsInternal();
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                throw new Exception("Error ensuring database exists: " + ex.Message, ex);
            }
        }

        private static void EnsureDatabaseExistsInternal()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                var databaseName = builder.InitialCatalog;

                builder.InitialCatalog = "master";

                var sql = GetInicializadorBdScript();

                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();

                    cmd.CommandText = @"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @dbName)
                    BEGIN
                        EXEC('CREATE DATABASE [' + @dbName + ']')
                    END";
                    
                    cmd.Parameters.AddWithValue("@dbName", databaseName);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking database: " + ex.Message, ex);
            }
        }

        private static void InitializeDatabase()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();

                    cmd.CommandText = GetInicializadorBdScript();

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing database: " + ex.Message, ex);
            }
        }

        private static string GetInicializadorBdScript()
        {
            var assembly = typeof(DAO).Assembly;
            using (var stream = assembly.GetManifestResourceStream("DAL.Scripts.InicializadorBD.sql"))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("SQL script not found in embedded resources.");
                }
                using (var reader = new StreamReader(stream))
                {
                    var sqlScript = reader.ReadToEnd();
                    return sqlScript;
                }
            }
        }
    }
}