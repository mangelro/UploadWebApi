/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 05/09/2019 12:45:47
 *
 */

using System;


namespace UploadWebApi.Applicacion.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfiguracionRegistro : IConfiguracionRegistros
    {
        public string RutaFicheros => "C:\\ArchivosCDF";
        public string RutaTemporal => System.IO.Path.GetTempPath();


    }
}