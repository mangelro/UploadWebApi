using System.IO;

namespace UploadWebApi.Infraestructura.SqlBinaryStream
{

    /// <summary>
    /// http://florianreischl.blogspot.com/2011/08/streaming-sql-server-varbinarymax-in.html
    /// </summary>

    interface IBinaryData
    {
        Stream OpenRead();
        Stream OpenWrite(bool append);
    }
}
