using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;
using BE;

namespace DAL
{
    public class DVDAL
    {
        private readonly DAO _dao = new DAO();

        public IEnumerable<DV> ObtenerTodos()
        {
            var dvs = _dao.Query("SELECT * FROM DV", MapFn);
            return dvs;
        }

        public DataTable ObtenerTabla(string tabla)
        {
            var dt = _dao.ExecuteDataset($"SELECT * FROM {tabla}");
            dt.TableName = tabla;
            return dt;
        }

        private static DV MapFn(SqlDataReader reader)
        {
            string tableName = reader.GetString(0);
            byte[] hashBytes = reader.GetSqlBytes(1).Value;

            var hash = DeserializeJaggedArray(hashBytes);

            return new DV(tableName, hash);
        }

        public void Guardar(DV dv)
        {
            const string cmdText = @"
                MERGE DV AS target
                USING (SELECT @TableName AS TableName, @Hash AS Hash) AS source
                    ON target.TableName = source.TableName
                WHEN MATCHED THEN
                    UPDATE SET Hash = source.Hash
                WHEN NOT MATCHED THEN
                    INSERT (TableName, Hash)
                    VALUES (source.TableName, source.Hash);";

            _dao.ExecuteNonQuery(cmdText, 
                new
                {
                    dv.TableName, 
                    Hash = SerializeJaggedArray(dv.Hash)
                });
        }

        private static byte[] SerializeJaggedArray(long[][] jaggedArray)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(jaggedArray.Length);
                foreach (long[] innerArray in jaggedArray)
                {
                    writer.Write(innerArray.Length);
                    foreach (long num in innerArray)
                    {
                        writer.Write(num);
                    }
                }
                return ms.ToArray();
            }
        }

        private static long[][] DeserializeJaggedArray(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var reader = new BinaryReader(ms))
            {
                int outerLength = reader.ReadInt32();
                long[][] jaggedArray = new long[outerLength][];

                for (int i = 0; i < outerLength; i++)
                {
                    int innerLength = reader.ReadInt32();
                    jaggedArray[i] = new long[innerLength];
                    for (int j = 0; j < innerLength; j++)
                    {
                        jaggedArray[i][j] = reader.ReadInt64();
                    }
                }
                return jaggedArray;
            }
        }
    }
}