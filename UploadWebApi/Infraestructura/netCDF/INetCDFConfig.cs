/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 18/09/2019 13:29:45
 *
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UploadWebApi.Infraestructura.Configuracion;

namespace UploadWebApi.Infraestructura.netCDF
{
    /// <summary>
    /// 
    /// </summary>
    public interface INetCDFConfig
    {

        string RutaNcdump { get; }
    }
}