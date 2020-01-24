/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 18/09/2019 13:31:02
 *
 */

using System;
using System.Configuration;

namespace UploadWebApi.Infraestructura.Configuracion
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationManagerHelper
    {
 

        public static string GetAppConfig(string configName, string defaultValue)
        {



            string val = ConfigurationManager.AppSettings[configName];

            return (String.IsNullOrEmpty(val) ? defaultValue : val);
        }
    }
}