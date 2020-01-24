/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 11/09/2019 15:24:48
 *
 */

using System.IO;
using System.IO.Compression;


namespace UploadWebApi.Infraestructura.Compresion
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class GZipCompresionService : IFiltroCompresion
    {


        public Stream Comprimir(Stream rawData)
        {
            var stream = new MemoryStream();

            using (var zipStream = new GZipStream(stream, CompressionMode.Compress))
            {
                rawData.CopyTo(zipStream);
            }
            stream.Flush();


            return stream;
        }

        public Stream Descomprimir(Stream compressData)
        {
            var outputStream = new MemoryStream();

            using (var zipStream = new GZipStream(compressData, CompressionMode.Decompress))
            {
                zipStream.CopyTo(outputStream);
            }
            outputStream.Flush();

            return outputStream;


        }
    }
}