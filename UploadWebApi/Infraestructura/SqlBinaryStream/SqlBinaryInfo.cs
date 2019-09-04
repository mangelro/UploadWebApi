using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Collections.Concurrent;

namespace UploadWebApi.Infraestructura.SqlBinaryStream
{

    internal class SqlBinaryInfo {
      /// <summary>
      /// meta data cache to avoid establishing meta data for each single row
      /// </summary>
      private static ConcurrentDictionary<SqlBinaryMetaData, SqlBinaryMetaData> _metaDataCache
         = new ConcurrentDictionary<SqlBinaryMetaData, SqlBinaryMetaData>();
      private static object _syncRoot = new object();

      private SqlBinaryMetaData _metaDataKey;
      private SqlParameter _pkParam;
      private object _pkValue;

      public SqlBinaryInfo(string connectionString,
                               string tableName,
                               string tableSchema,
                               string binaryColumn,
                               SqlParameter pkParam,
                               object pkValue,
                               int bufferSize) {
         if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException("connectionString");
         if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException("tableName");
         if (string.IsNullOrEmpty(binaryColumn))
            throw new ArgumentNullException(binaryColumn);
         if (pkParam == null)
            throw new ArgumentNullException("pkParam");
         if (pkValue == null)
            throw new ArgumentNullException("pkValue");
         if (bufferSize <= 0)
            throw new ArgumentException("Parameter bufferSize must be a positive integer value", "bufferSize");

         _pkValue = pkValue;
         _pkParam = pkParam;
         BufferSize = bufferSize;

         _metaDataKey = new SqlBinaryMetaData
                           { 
                              ConnectionString = connectionString,
                              BinaryColumn = binaryColumn,
                              TableName = tableName,
                              TableSchema = tableSchema,
                           };
      }

      public static void ClearMetaDataCache() {
         lock (_syncRoot) {
            _metaDataCache = new ConcurrentDictionary<SqlBinaryMetaData, SqlBinaryMetaData>();
         }
      }

      public int BufferSize { get; private set; }

      public SqlConnection CreateConnection(string connectionString) {
         SqlConnection cn = new SqlConnection(connectionString);
         cn.Open();
         return cn;
      }

      public SqlParameter CreatePkParam() {
         SqlParameter param = (SqlParameter)((ICloneable)_pkParam).Clone();
         param.ParameterName = "@pk";
         param.Value = _pkValue;
         return param;
      }

      public void AssertOneRowAffected(int affected) {
         if (affected != 1)
            throw new DataException(
               string.Format("Expected 1 row to be affected but was {0}", affected));
      }

      public SqlBinaryMetaData GetMetaData() {
         SqlBinaryMetaData metaData;
         if (!_metaDataCache.TryGetValue(_metaDataKey, out metaData)) {
            lock (_syncRoot) {
               if (!_metaDataCache.TryGetValue(_metaDataKey, out metaData)) {
                  metaData = CreateMetaData();
                  _metaDataCache.TryAdd(_metaDataKey, metaData);
               }
            }
         }
         return metaData;
      }

      /// <summary>
      /// Creates safe names for all provided table and column names by utilizing SQL Server
      /// QUOTENAME() function. This is done to avoid SQL injection.
      /// </summary>
      private SqlBinaryMetaData CreateMetaData() {
         SqlBinaryMetaData metaData = 
            new SqlBinaryMetaData { ConnectionString = _metaDataKey.ConnectionString };
         StringWriter sql = new StringWriter();

         try {
            using (var cn = CreateConnection(_metaDataKey.ConnectionString)) {
               string tableSchema;
               GetTableAndSchemaName(metaData, cn, out tableSchema);
               GetPrimaryKeyColumn(metaData, cn, tableSchema);
               GetBinaryColumn(metaData, cn, tableSchema);
            }
         }
         catch {
            throw;
         }

         return metaData;
      }

      private void GetTableAndSchemaName(SqlBinaryMetaData metaData, SqlConnection cn, out string tableSchema) {
         // validate and quote table and schema name
         string sql = @"SELECT QUOTENAME(TABLE_NAME), QUOTENAME(TABLE_SCHEMA), TABLE_SCHEMA
                        FROM INFORMATION_SCHEMA.TABLES
                        WHERE TABLE_NAME = @tableName ";

         // if provided add schema information, otherwise use default schema for table
         // to avoid accessing a wrong table in another schema
         if (string.IsNullOrEmpty(_metaDataKey.TableSchema))
            sql += "AND TABLE_SCHEMA = OBJECT_SCHEMA_NAME(OBJECT_ID(@tableName))";
         else
            sql += "AND TABLE_SCHEMA = @schemaName";

         using (var cmd = new SqlCommand(sql.ToString(), cn)) {
            cmd.Parameters.Add("@tableName", SqlDbType.NVarChar, 255).Value = _metaDataKey.TableName;
            if (!string.IsNullOrEmpty(_metaDataKey.TableSchema))
               cmd.Parameters.Add("@schemaName", SqlDbType.NVarChar, 255).Value = _metaDataKey.TableSchema;
            using (var reader = cmd.ExecuteReader()) {
               if (!reader.Read())
                  throw new DataException(
                     string.Format("Cannot find table information for table '{0}' and schema '{0}'",
                                   _metaDataKey.TableName,
                                   _metaDataKey.TableSchema));
               metaData.TableName = (string)reader[0];
               metaData.TableSchema = (string)reader[1];
               tableSchema = (string)reader[2];
            }
         }
      }

      private void GetPrimaryKeyColumn(SqlBinaryMetaData metaData, SqlConnection cn, string tableSchema) {
         // validate and get primary key column
         string sql = @"SELECT QUOTENAME(COLUMN_NAME)
                        FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
                        WHERE TABLE_NAME = @tableName AND TABLE_SCHEMA = @schemaName
                        AND OBJECTPROPERTY(OBJECT_ID(QUOTENAME(CONSTRAINT_SCHEMA)
                                 + '.' + QUOTENAME(CONSTRAINT_NAME))
                              ,'IsPrimaryKey') = 1";

         using (var cmd = new SqlCommand(sql.ToString(), cn)) {
            cmd.Parameters.Add("@tableName", SqlDbType.NVarChar, 255).Value = _metaDataKey.TableName;
            cmd.Parameters.Add("@schemaName", SqlDbType.NVarChar, 255).Value = tableSchema;
            using (var reader = cmd.ExecuteReader()) {
               if (!reader.Read())
                  throw new DataException(
                     string.Format("Table '{0}' in schema '{1}' does not have a primary key",
                                   _metaDataKey.TableName,
                                   _metaDataKey.TableSchema));
               metaData.PkColumn = (string)reader[0];

               if (reader.Read())
                  throw new DataException(
                     string.Format("Table '{0}' in schema '{1}' has more than one primary key column.",
                                   _metaDataKey.TableName,
                                   _metaDataKey.TableSchema));
            }
         }
      }

      private void GetBinaryColumn(SqlBinaryMetaData metaData, SqlConnection cn, string tableSchema) {
         // validate and quote binary column
         string sql = @"SELECT QUOTENAME(COLUMN_NAME), DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_NAME = @tableName
                           AND TABLE_SCHEMA = @schemaName
                           AND COLUMN_NAME = @columnName";

         using (var cmd = new SqlCommand(sql.ToString(), cn)) {
            cmd.Parameters.Add("@tableName", SqlDbType.NVarChar, 255).Value = _metaDataKey.TableName;
            cmd.Parameters.Add("@schemaName", SqlDbType.NVarChar, 255).Value = tableSchema;
            cmd.Parameters.Add("@columnName", SqlDbType.NVarChar, 255).Value = _metaDataKey.BinaryColumn;
            using (var reader = cmd.ExecuteReader()) {
               if (!reader.Read())
                  throw new DataException(
                     string.Format("Table '{0}' in schema '{1}' does not have a primary key",
                                   _metaDataKey.TableName,
                                   _metaDataKey.TableSchema));
               metaData.BinaryColumn = (string)reader[0];
               string dataType = ((string)reader[1]).ToLower();
               object value = reader[2];
               int maxLenth = value is DBNull ? 0 : (int)value;
               if (dataType != "varbinary" || maxLenth != -1)
                  throw new DataException(
                     string.Format("Invalid binary column type '{0}'. Column must be VARBINARY(MAX)", dataType));
            }
         }
      }
   }
}
