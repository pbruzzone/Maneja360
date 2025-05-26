using System;
using System.Collections.Generic;
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

        public IEnumerable<T> Query<T>(string cmdText, Func<IDataReader, T> map, object param = null)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    SetupCommand(cmd, cmdText, param);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return map(reader);
                        }
                    }
                }
            }
        }

        public void ExecuteNonQuery(string cmdText, object param = null)
        {
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    SetupCommand(cmd, cmdText, param);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public T ExecuteScalar<T>(string cmdText, object param = null)
        {
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    SetupCommand(cmd, cmdText, param);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return (T)result;
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