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
    /// 
    /// </summary>
    public class FakeCompresionService : IFiltroCompresion
    {
        public byte[] Comprimir(byte[] rawData)
        {
            return rawData;
        }

        public byte[] Descomprimir(byte[] compressData)
        {
            return compressData;
        }
    }
}