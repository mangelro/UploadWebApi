using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// ------------------------------------------------------------------
/// <sumary>
/// 
/// 
/// </sumary>
/// <copyright file="ClaimTypes">
///     ©2018. Todos los derechos reservados.
/// </copyright>
/// <author>Miguel Á. Romera [miguel]</author>
/// <date>04/10/2018 9:18:21</date>
/// ------------------------------------------------------------------
///


namespace UploadWebApi.Infraestructura.Seguridad
{
    public static class ClaimTypes
    {
        public const string Permiso = "http://schemas.microsoft.com/ws/2018/06/identity/claims/permiso";
        public const string AppId = "http://schemas.microsoft.com/ws/2018/06/identity/claims/appid";


    }// ClaimTypes
}