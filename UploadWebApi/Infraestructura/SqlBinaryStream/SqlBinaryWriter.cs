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
    /// Internal writer stream returned by <see cref="SqlBinaryData.OpenWrite"/>
    /// </summary>
    class SqlBinaryWriter : Stream {
      private SqlBinaryInfo _info;
      private SqlBinaryMetaData _metaData;
      private bool _append;
      private int _position;
      private bool _prepared;
      private int _internalOffset;
      private byte[] _internalBuffer;
      private bool _failedState;
      private SqlConnection _cn;
      private SqlTransaction _tran;

      public SqlBinaryWriter(SqlBinaryInfo info, bool append) {
         _info = info;
         _internalBuffer = new byte[_info.BufferSize];
         _append = append;
      }

      public override bool CanRead { get { return false; } }
      public override bool CanSeek { get { return false; } }
      public override bool CanWrite { get { return true; } }

      protected override void Dispose(bool disposing) {
         base.Dispose(disposing);
         Flush();
      }

      public override void Flush() {
         if (_internalOffset == 0)
            return;
         if (_failedState)
            return;

         SqlBinaryMetaData metaData = GetMetaData();

         using (SqlConnection cn = _info.CreateConnection(metaData.ConnectionString))
         using (var tran = cn.BeginTransaction()) {
            try {
               // handle NULL value and "append" configuration
               PrepareValue(cn, tran);
               // UPDATE SchemaName.TableName 
               // SET BinaryColumn.WRITE(@buffer, @offset, @count) 
               // WHERE PkColumn = @pk
               string sql =
                  string.Format("UPDATE {0} SET {1}.WRITE(@buffer, @offset, @count) WHERE {2} = @pk",
                                metaData.QualifiedTableName,
                                metaData.BinaryColumn,
                                metaData.PkColumn);
               using (var cmd = new SqlCommand(sql, cn)) {
                  cmd.Transaction = tran;

                  var bufferParam = cmd.Parameters.Add("@buffer", SqlDbType.VarBinary, _info.BufferSize);
                  var offsetParam = cmd.Parameters.Add("@offset", SqlDbType.Int);
                  var countParam = cmd.Parameters.Add("@count", SqlDbType.Int);
                  cmd.Parameters.Add(_info.CreatePkParam());

                  byte[] buffer;
                  if (_internalOffset == _internalBuffer.Length)
                     buffer = _internalBuffer;
                  else {
                     // avoid bumping not needed data over network
                     buffer = new byte[_internalOffset];
                     Array.Copy(_internalBuffer, buffer, _internalOffset);
                  }

                  bufferParam.Value = buffer;
                  // VARBINARY(MAX).WRITE works with a zero based index
                  offsetParam.Value = _position;
                  countParam.Value = _internalOffset;
                  // write chunk
                  int affected = cmd.ExecuteNonQuery();
                  _info.AssertOneRowAffected(affected);
                  _position += _internalOffset;
                  _internalOffset = 0;
               }
               tran.Commit();
            }
            catch {
               _failedState = true;
               tran.Rollback();
               throw;
            }
         }
      }

      public override long Length { get { throw new NotSupportedException(); } }

      public override long Position {
         get { return _position; }
         set { throw new NotSupportedException(); }
      }

      public override int Read(byte[] buffer, int offset, int count) {
         throw new NotSupportedException();
      }

      public override long Seek(long offset, SeekOrigin origin) {
         throw new NotSupportedException();
      }

      public override void SetLength(long value) {
         throw new NotSupportedException();
      }

      public override void Write(byte[] buffer, int offset, int count) {
         if (_failedState)
            throw new InvalidOperationException("Stream is in a failed state");

         int done = 0;
         while (done != count) {
            int chunk = Math.Min(_internalBuffer.Length - _internalOffset, count - done);
            // push a chunk of bytes into the internal buffer
            Array.Copy(buffer, offset + done, _internalBuffer, _internalOffset, chunk);
            _internalOffset += chunk;
            // if internal buffer is full, flush to database
            if (_internalOffset == _internalBuffer.Length)
               Flush();
            done += chunk;
         }
      }

      private SqlBinaryMetaData GetMetaData() {
         return _metaData ?? (_metaData = _info.GetMetaData());
      }

      /// <summary>
      /// Ensures that field in database is not NULL, gets the current position
      /// in case of appending is configured and resets an existing value if 
      /// append was set to "false"
      /// </summary>
      /// <param name="cn">An active database connection</param>
      /// <param name="tran">The current transaction</param>
      private void PrepareValue(SqlConnection cn, SqlTransaction tran) {
         if (_prepared)
            return;
         SqlBinaryMetaData metaData = GetMetaData();
         using (var cmd = new SqlCommand()) {
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.Parameters.Add(_info.CreatePkParam());

            if (_append) {
               // handle NULL values and get the current length as start position
               // the SQL statement will automatically create an exception if more than
               // one row was affected, no additional handling needed
               cmd.CommandText =
                  string.Format(@"
DECLARE @offset INT, @check BIT;
SET @check = 0;
SELECT @check = 1, @offset = DATALENGTH({0}) FROM {1} WHERE {2} = @pk;
IF (@check = 1 AND @offset IS NULL)
   UPDATE {1} SET {0} = 0x WHERE {2} = @pk;
SELECT ISNULL(@offset, 0), @check;",
                                       metaData.BinaryColumn,
                                       metaData.QualifiedTableName,
                                       metaData.PkColumn);
               using (var reader = cmd.ExecuteReader()) {
                  reader.Read();
                  _position = (int)reader[0];
                  bool check = (bool)reader[1];
                  if (!check)
                     throw new DataException("Row to write to was not found.");
               }
            }
            else {
               // reset the current value of the column
               cmd.CommandText =
                  string.Format("UPDATE {0} SET {1} = 0x WHERE {2} = @pk",
                                metaData.QualifiedTableName,
                                metaData.BinaryColumn,
                                metaData.PkColumn);
               int affected = cmd.ExecuteNonQuery();
               _info.AssertOneRowAffected(affected);
               _position = 0;
            }
         }
         _prepared = true;
      }
   }
}
