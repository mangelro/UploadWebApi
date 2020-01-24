/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 11/09/2019 15:24:48
 *
 */

using System;
using System.IO;

namespace UploadWebApi.Infraestructura.Compresion
{
    /// <summary>
    /// 
    /// </summary>
    public class FakeCompresionService : IFiltroCompresion
    {
        public Stream Comprimir(Stream rawData)
        {
            return rawData;
        }

        public Stream Descomprimir(Stream compressData)
        {
            return compressData;
        }
    }
}