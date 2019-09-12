/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 11/09/2019 15:24:48
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadWebApi.Applicacion.Stores;

namespace UploadWebApi.Infraestructura.Servicios
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class GZipCompresionService : IFiltroCompresion
    {
        public byte[] Comprimir(byte[] rawData)
        {
            //byte[] buffer = new byte[512];
            //int leidos = 0;


            using (var inputStream = new MemoryStream(rawData, false))
            {
                using (var stream = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(stream, CompressionMode.Compress))
                    {
                        inputStream.CopyTo(zipStream);
                    }
                    stream.Flush();
                    return stream.ToArray();
                }
            }
        }

        public byte[] Descomprimir(byte[] compressData)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(new MemoryStream(compressData, false), CompressionMode.Decompress))
                {
                    zipStream.CopyTo(outputStream);
                }
                outputStream.Flush();
                return outputStream.ToArray();
            }

        }
    }
}