/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 11/09/2019 15:22:01
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Applicacion.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFiltroCompresion
    {

        byte[] Comprimir(byte[] rawData);


        byte[] Descomprimir(byte[] compressData);
    }
}