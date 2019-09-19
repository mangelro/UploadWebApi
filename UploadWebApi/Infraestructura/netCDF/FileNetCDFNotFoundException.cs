/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 18/09/2019 9:50:08
 *
 */

using System;

namespace UploadWebApi.Infraestructura.netCDF
{
    /// <summary>
    /// 
    /// </summary>
    public class FileNetCDFNotFoundException : NetCDFException
    {

        public FileNetCDFNotFoundException(string message) : base(message, new FileNetCDFNotFoundException(message))
        {
        }

    }
}