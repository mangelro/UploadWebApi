/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 11/09/2019 15:22:01
 *
 */

using System;
using System.IO;

namespace UploadWebApi.Infraestructura.Compresion
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFiltroCompresion
    {

        Stream Comprimir(Stream rawData);


        Stream Descomprimir(Stream compressData);
    }
}