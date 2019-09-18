/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 18/09/2019 9:50:08
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Infraestructura.netCDF
{
    /// <summary>
    /// 
    /// </summary>
    public class NetCDFException:Exception
    {

        public NetCDFException(string message):base(message)
        {
        }

    }
}