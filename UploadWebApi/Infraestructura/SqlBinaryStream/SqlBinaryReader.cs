using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace UploadWebApi.Infraestructura.SqlBinaryStream
{
    /// <summary>
    /// Internal reader stream returned by <see cref="SqlBinaryData.OpenRead"/>
    /// </summary>
    class SqlBinaryReader : Stream {
      private SqlBinaryInfo _info;
      private byte[] _buffer;
      private int _offset;
      private int _position;
      private SqlConnection _cn;
      private SqlDataReader _reader;
      private SqlBinaryMetaData _metaData;

      public SqlBinaryReader(SqlBinaryInfo info) {
         _info = info;
         _offset = info.BufferSize;
         _buffer = new byte[_offset];
      }

      public override bool CanRead { get { return true; } }
      public override bool CanSeek { get { return false; } }
      public override bool CanWrite { get { return false; } }

      public override void Flush() { }

      public override long Length {
         get { throw new NotSupportedException(); }
      }

      public override long Position {
         get { return _position; }
         set { throw new NotSupportedException(); }
      }

      public override long Seek(long offset, SeekOrigin origin) {
         throw new NotSupportedException();
      }

      public override void SetLength(long value) {
         throw new NotSupportedException();
      }

      public override void Write(byte[] buffer, int offset, int count) {
         throw new NotSupportedException();
      }

      public override int Read(byte[] buffer, int offset, int count) {
         int done = 0;

         while (count != done) {
            // read buffered data into provided buffer
            done += ReadInternal(buffer, offset + done, count - done);
            // end of DB data reached
            if (_buffer.Length < _info.BufferSize)
               break;
            // read next chunk from database if needed
            if (done < count)
               ReadChunk();
         }

         return done;
      }

      protected override void Dispose(bool disposing) {
         base.Dispose(disposing);
         if (_reader != null) {
            _reader.Dispose();
            _reader = null;
         }
         if (_cn != null) {
            _cn.Dispose();
            _cn = null;
         }
      }

      private int ReadInternal(byte[] buffer, int offset, int count) {
         int available = Math.Min(_buffer.Length - _offset, count);
         if (available != 0)
            Array.Copy(_buffer, _offset, buffer, offset, available);
         _offset += available;
         _position += available;
         return available;
      }

      private SqlBinaryMetaData GetMetaData() {
         return _metaData ?? (_metaData = _info.GetMetaData());
      }

      private void ReadChunk() {
         SqlBinaryMetaData metaData = GetMetaData();
         // create an internal database connection if not yet available
         if (_cn == null)
            _cn = _info.CreateConnection(GetMetaData().ConnectionString);

         // create an internal data reader
         if (_reader == null) {
            string sql =
               string.Format("SELECT {0} FROM {1} WHERE {2} = @pk",
                             metaData.BinaryColumn,
                             metaData.QualifiedTableName,
                             metaData.PkColumn);
            using (var cmd = new SqlCommand(sql, _cn)) {
               cmd.Parameters.Add(_info.CreatePkParam());
               // open the reader with sequencial access behavior to enable 
               // streaming data from database
               _reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
               _reader.Read();
            }
         }

         int read = (int)_reader.GetBytes(0, _position, _buffer, 0, _buffer.Length);
         if (read != _buffer.Length)
            Array.Resize(ref _buffer, read);
         _offset = 0;
      }
   }
}
