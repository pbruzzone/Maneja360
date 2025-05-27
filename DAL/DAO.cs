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
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["Maneja360"].ConnectionString;

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public IEnumerable<T> Query<T>(string cmdText, object param = null, Action<T> callbackFn = null)
        {
            return Query(cmdText, MapDataReaderTo<T>, param, callbackFn);
        }

        public IEnumerable<T> Query<T>(string cmdText, Func<IDataReader, T> map, object param = null, Action<T> callbackFn = null)
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
                            var obj = map(reader);

                            callbackFn?.Invoke(obj);

                            yield return obj;
                        }
                    }
                }
            }
        }

        public T QuerySingleOrDefault<T>(string cmdText, object param = null, Action<T> callbackFn = null)
        {
            return QuerySingleOrDefault(cmdText, MapDataReaderTo<T>, param, callbackFn);
        }

        public T QuerySingleOrDefault<T>(string cmdText, Func<IDataReader, T> map, object param = null, Action<T> callbackFn = null)
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
                        int readCount = 0;
                        T obj = default(T);

                        while (reader.Read())
                        {
                            readCount++;
                            if (readCount > 1)
                            {
                                throw new InvalidOperationException("La consulta devolvió más de un elemento.");
                            }

                            obj = map(reader);

                            callbackFn?.Invoke(obj);
                        }

                        return obj;
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
       
        private static T MapDataReaderTo<T>(IDataReader reader)  
        {
            var obj = Activator.CreateInstance<T>();
            var type = typeof(T);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                if (name.Contains("_"))
                {
                    var parts = name.Split('_');
                    var parentProp = type.GetProperty(parts[0]);
                    
                    if (parentProp == null || !parentProp.CanWrite) continue;
                    
                    var childObj = parentProp.GetValue(obj) ?? Activator.CreateInstance(parentProp.PropertyType);
                    var childProp = parentProp.PropertyType.GetProperty(parts[1]);
                    
                    if (childProp == null || !childProp.CanWrite) continue;
                    
                    var childValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    childProp.SetValue(childObj, childValue);
                    parentProp.SetValue(obj, childObj);
                }
                else
                {
                    var prop = type.GetProperty(name);
                    
                    if (prop == null || !prop.CanWrite) continue;
                    
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    prop.SetValue(obj, value);
                }
            }
            return obj;
        }
    }
}