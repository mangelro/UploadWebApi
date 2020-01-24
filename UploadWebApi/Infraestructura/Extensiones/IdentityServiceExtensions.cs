/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 24/01/2020 16:09:33
 *
 */

using System.Linq;

using UploadWebApi.Aplicacion.Servicios;

namespace UploadWebApi.Infraestructura.Extensiones
{

    /// <summary>
    /// 
    /// </summary>
    public static class IdentityServiceExtensions
    {
        const string ROL_CONTRASTADOR = "contrastadores";



        public static bool EsContrastadorOAdministrador(this IIdentityService service)
        {

            return (service.IsSysAdmin || service.Roles.Where(r => r == ROL_CONTRASTADOR).Any());


        }
    }
}