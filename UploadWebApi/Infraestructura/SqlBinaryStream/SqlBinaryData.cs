using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace UploadWebApi.Infraestructura.SqlBinaryStream
{
    /// <summary>
    /// Represets a class that provides streaming read and/or write access to SQL Server
    /// VARBINARY(MAX) columns.
    /// </summary>
    /// <remarks>
    /// To avoid SQL injection, all provided table and column names are quoted by SQL Server
    /// function QUOTENAME() before first usage. Do NOT provide custom quoted names as this
    /// causes invalid table and column names.<br/>
    /// <br/>
    /// To get a good execution plan caching and avoid SQL injection all SQL statements using 
    /// parameterized values.
    /// </remarks>
    public class SqlBinaryData : IBinaryData
    {



        private SqlBinaryInfo _info;

        /// <summary>
        /// Creates a new instance of a streaming supporting class for SQL Server 
        /// VARBINARY(MAX)  columns
        /// </summary>
        /// <param name="connectionString">The connection string that provides access to 
        /// the database where the source table is stored.</param>
        /// <param name="tableName">The name of the source table of BLOB colum</param>
        /// <param name="tableSchema">The name of the tables schema or null in case of 
        /// the default schema.</param>
        /// <param name="binaryColumn">The name of the column that stores the BLOB data.</param>
        /// <param name="pkParam">A configured <see cref="SqlParameter"/> that represents
        /// the column definition of the primary key column.</param>
        /// <param name="pkValue">The value of the primary key that identifies the 
        /// row of the BLOB to be streamed.</param>
        /// <param name="bufferSize">The buffer size for created streams.</param>
        /// <remarks>At the moment only single column primary keys are supported.<br/>
        /// <br/>
        /// The provided <see cref="SqlParameter"/> <paramref name="pkParam"/> will be cloned
        /// inside of the class. Any further changes of the outside parameter will not take effect
        /// to the internal parameter after calling the constructor.</remarks>
        public SqlBinaryData(string connectionString, string tableName, string tableSchema,
                             string binaryColumn, SqlParameter pkParam, object pkValue, int bufferSize)
        {
            _info = new SqlBinaryInfo(connectionString, tableName, tableSchema, binaryColumn, pkParam, pkValue, bufferSize);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="SqlBinaryData"/>, configured for a SQL Server INT column.
        /// </summary>
        /// <param name="connectionString">The connection string that provides access to the database
        /// where the source table is stored.</param>
        /// <param name="tableName">The name of the source table of BLOB colum</param>
        /// <param name="tableSchema">The name of the tables schema or null in case of the default schema.</param>
        /// <param name="binaryColumn">The name of the column that stores the BLOB data.</param>
        /// <param name="pkValue">The value of the primary key that identifies the row of the BLOB
        /// to be streamed.</param>
        /// <param name="bufferSize">The buffer size for created streams.</param>
        /// <returns>A new instance of a <see cref="SqlBinaryData"/> to stream binary data
        /// to or from SQL Server</returns>
        public static SqlBinaryData CreateIntPrimaryKey(string connectionString, string tableName, string tableSchema,
                                                        string binaryColumn, int pkValue, int bufferSize)
        {
            SqlParameter pkParam = new SqlParameter("@pk", SqlDbType.Int);
            return new SqlBinaryData(connectionString, tableName, tableSchema, binaryColumn, pkParam, pkValue, bufferSize);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="SqlBinaryData"/>, configured for a SQL Server INT column.
        /// </summary>
        /// <param name="connectionString">The connection string that provides access to the database
        /// where the source table is stored.</param>
        /// <param name="tableName">The name of the source table of BLOB colum</param>
        /// <param name="binaryColumn">The name of the column that stores the BLOB data.</param>
        /// <param name="pkValue">The value of the primary key that identifies the row of the BLOB
        /// to be streamed.</param>
        /// <param name="bufferSize">The buffer size for created streams.</param>
        /// <returns>A new instance of a <see cref="SqlBinaryData"/> to stream binary data
        /// to or from SQL Server</returns>
        public static SqlBinaryData CreateIntPrimaryKey(string connectionString, string tableName,
                                                        string binaryColumn, int pkValue, int bufferSize)
        {
            SqlParameter pkParam = new SqlParameter("@pk", SqlDbType.Int);
            return new SqlBinaryData(connectionString, tableName, null, binaryColumn, pkParam, pkValue, bufferSize);
        }

        /// Creates a new instance of a <see cref="SqlBinaryData"/>, configured for a SQL Server INT column.
        /// </summary>
        /// <param name="connectionString">The connection string that provides access to the database
        /// where the source table is stored.</param>
        /// <param name="tableName">The name of the source table of BLOB colum</param>
        /// <param name="tableSchema">The name of the tables schema or null in case of the default schema.</param>
        /// <param name="binaryColumn">The name of the column that stores the BLOB data.</param>
        /// <param name="pkValue">The value of the primary key that identifies the row of the BLOB
        /// to be streamed.</param>
        /// <param name="bufferSize">The buffer size for created streams.</param>
        /// <returns>A new instance of a <see cref="SqlBinaryData"/> to stream binary data
        /// to or from SQL Server</returns>
        public static SqlBinaryData CreateLongPrimaryKey(string connectionString, string tableName, string tableSchema,
                                                         string binaryColumn, long pkValue, int bufferSize)
        {
            SqlParameter pkParam = new SqlParameter("@pk", SqlDbType.BigInt);
            return new SqlBinaryData(connectionString, tableName, tableSchema, binaryColumn, pkParam, pkValue, bufferSize);
        }

        /// Creates a new instance of a <see cref="SqlBinaryData"/>, configured for a SQL Server INT column.
        /// </summary>
        /// <param name="connectionString">The connection string that provides access to the database
        /// where the source table is stored.</param>
        /// <param name="tableName">The name of the source table of BLOB colum</param>
        /// <param name="binaryColumn">The name of the column that stores the BLOB data.</param>
        /// <param name="pkValue">The value of the primary key that identifies the row of the BLOB
        /// to be streamed.</param>
        /// <param name="bufferSize">The buffer size for created streams.</param>
        /// <returns>A new instance of a <see cref="SqlBinaryData"/> to stream binary data
        /// to or from SQL Server</returns>
        public static SqlBinaryData CreateLongPrimaryKey(string connectionString, string tableName,
                                                         string binaryColumn, long pkValue, int bufferSize)
        {
            SqlParameter pkParam = new SqlParameter("@pk", SqlDbType.BigInt);
            return new SqlBinaryData(connectionString, tableName, null, binaryColumn, pkParam, pkValue, bufferSize);
        }

        /// Creates a new instance of a <see cref="SqlBinaryData"/>, configured for a SQL Server INT column.
        /// </summary>
        /// <param name="connectionString">The connection string that provides access to the database
        /// where the source table is stored.</param>
        /// <param name="tableName">The name of the source table of BLOB colum</param>
        /// <param name="binaryColumn">The name of the column that stores the BLOB data.</param>
        /// <param name="pkValue">The value of the primary key that identifies the row of the BLOB
        /// to be streamed.</param>
        /// <param name="bufferSize">The buffer size for created streams.</param>
        /// <returns>A new instance of a <see cref="SqlBinaryData"/> to stream binary data
        /// to or from SQL Server</returns>
        public static SqlBinaryData CreateGuidPrimaryKey(string connectionString, string tableName,
                                                         string binaryColumn, Guid pkValue, int bufferSize)
        {
            SqlParameter pkParam = new SqlParameter("@pk", SqlDbType.UniqueIdentifier);
            return new SqlBinaryData(connectionString, tableName, null, binaryColumn, pkParam, pkValue, bufferSize);
        }

        /// Creates a new instance of a <see cref="SqlBinaryData"/>, configured for a SQL Server INT column.
        /// </summary>
        /// <param name="connectionString">The connection string that provides access to the database
        /// where the source table is stored.</param>
        /// <param name="tableName">The name of the source table of BLOB colum</param>
        /// <param name="tableSchema">The name of the tables schema or null in case of the default schema.</param>
        /// <param name="binaryColumn">The name of the column that stores the BLOB data.</param>
        /// <param name="pkValue">The value of the primary key that identifies the row of the BLOB
        /// to be streamed.</param>
        /// <param name="bufferSize">The buffer size for created streams.</param>
        /// <returns>A new instance of a <see cref="SqlBinaryData"/> to stream binary data
        /// to or from SQL Server</returns>
        public static SqlBinaryData CreateGuidPrimaryKey(string connectionString, string tableName, string tableSchema,
                                                         string binaryColumn, Guid pkValue, int bufferSize)
        {
            SqlParameter pkParam = new SqlParameter("@pk", SqlDbType.UniqueIdentifier);
            return new SqlBinaryData(connectionString, tableName, tableSchema, binaryColumn, pkParam, pkValue, bufferSize);
        }

        public static void ClearMetaDataCache()
        {
            SqlBinaryInfo.ClearMetaDataCache();
        }

        /// <summary>
        /// Creates a new readable instance of a <see cref="Stream"/> for the BLOB column
        /// </summary>
        /// <returns>A readable stream.</returns>
        public Stream OpenRead()
        {
            return new SqlBinaryReader(_info);
        }

        /// <summary>
        /// Creates a new writable instance of a <see cref="Stream"/> for the BLOB column
        /// </summary>
        /// <returns>A writable stream.</returns>
        public Stream OpenWrite(bool append)
        {
            return new SqlBinaryWriter(_info, append);
        }
    }
}
