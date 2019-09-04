using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UploadWebApi.Infraestructura.SqlBinaryStream
{




    class SqlBinaryMetaData : IEquatable<SqlBinaryMetaData> {
      public string ConnectionString { get; set; }
      public string TableSchema { get; set; }
      public string TableName { get; set; }
      public string PkColumn { get; set; }
      public string BinaryColumn { get; set; }
      public string QualifiedTableName {
         get { return string.Format("{0}.{1}", TableSchema, TableName); }
      }

      public override int GetHashCode() {
         return ConnectionString.GetHashCode()
                + TableName.GetHashCode()
                + (TableSchema ?? string.Empty).GetHashCode()
                + (PkColumn ?? string.Empty).GetHashCode()
                + BinaryColumn.GetHashCode();
      }

      public override bool Equals(object obj) {
         return obj is SqlBinaryMetaData
                && Equals((SqlBinaryMetaData)obj);
      }

      public bool Equals(SqlBinaryMetaData other) {
         return string.Equals(other.ConnectionString, ConnectionString)
                && string.Equals(other.BinaryColumn, BinaryColumn)
                && string.Equals(other.PkColumn, PkColumn)
                && string.Equals(other.TableSchema, TableSchema)
                && string.Equals(other.TableName, TableName);
      }
   }
}
