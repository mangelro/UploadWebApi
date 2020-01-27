/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 05/09/2019 12:45:47
 *
 */

using System;
using System.Web;

using UploadWebApi.Aplicacion.Servicios;

namespace UploadWebApi.Infraestructura.Configuracion
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfiguracionRegistro : IConfiguracionRegistros
    {
        public string RutaFicheros => ConfigurationManagerHelper.GetAppConfig("appConfRutaFicheros", "C:\\ArchivosCDF");

        public string RutaTemporal => ConfigurationManagerHelper.GetAppConfig("appConfRutaTemporal", System.IO.Path.GetTempPath());

        public string RutaExeContraste => HttpContext.Current.Server.MapPath(ConfigurationManagerHelper.GetAppConfig("appConfRutaExeContraste", "~/App_Data/Contraste/ContrasteStub.exe"));

        public string NombreVector => ConfigurationManagerHelper.GetAppConfig("appNombreVector", "");
    }
}