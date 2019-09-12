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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadWebApi.Applicacion.Stores;

namespace UploadWebApi.Infraestructura.Servicios
{
    /// <summary>
    /// compresion mediante algoritmo LZMA compression and decompression
    /// https://www.7-zip.org/sdk.html - 2019-02-21	v 19.00
    /// </summary>
    public class SevenZipCompresionService : IFiltroCompresion
    {
        public byte[] Comprimir(byte[] rawData)
        {
            byte[] compressed = SevenZipHelper
                                .Compress(rawData);

            return compressed;
        }

        public byte[] Descomprimir(byte[] compressData)
        {
            byte[] decompressed = SevenZipHelper
                                    .Decompress(compressData);

            return decompressed;
        }
    }
}