using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Tipos = UploadWebApi.Infraestructura.Seguridad;

/// ------------------------------------------------------------------
/// <sumary>
/// 
/// 
/// </sumary>
/// <copyright file="IdentityExtensions">
///     ©2018. Todos los derechos reservados.
/// </copyright>
/// <author>Miguel Á. Romera [miguel]</author>
/// <date>09/03/2018 16:42:03</date>
/// ------------------------------------------------------------------
///


namespace UploadWebApi.Infraestructura.Extensiones
{

    public static class IdentityExtensions {

        public static Task<Guid> GetUserIdAsync(this IIdentity _identity)
        {
            Guid _retVal = Guid.Empty;
            try
            {
                if(_identity != null && _identity.IsAuthenticated)
                {
                    var ci = _identity as ClaimsIdentity;
                    string _userId = ci?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(_userId))
                    {
                        _retVal = Guid.Parse(_userId);
                    }
                }
                return Task.FromResult (_retVal);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public static Task<string> GetUserNameAsync(this IIdentity _identity)
        {
            string retVal="";
            try
            {
                if(_identity != null && _identity.IsAuthenticated)
                {
                    var ci = _identity as ClaimsIdentity;
                    string _userId = ci?.FindFirst(ClaimTypes.Name)?.Value;
                    retVal = _userId;
                }

                return Task.FromResult(retVal);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public static Task<Guid> GetAppClientIdAsync(this IIdentity _identity)
        {
            Guid _retVal = Guid.Empty;
            try
            {
                if(_identity != null && _identity.IsAuthenticated)
                {
                    var ci = _identity as ClaimsIdentity;
                    string _clientId = ci?.FindFirst(Tipos.ClaimTypes.AppId)?.Value;

                    if (!string.IsNullOrEmpty(_clientId))
                    {
                        _retVal = Guid.Parse(_clientId);
                    }
                }
                return Task.FromResult(_retVal);
            }
            catch(Exception)
            {
                throw;
            }
        }





        public static Guid GetUserId(this IIdentity _identity)
        {
            Guid retVal = Guid.Empty;
            try
            {
                if (_identity != null && _identity.IsAuthenticated)
                {
                    var ci = _identity as ClaimsIdentity;
                    string _userId = ci?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(_userId))
                    {
                        retVal = Guid.Parse(_userId);
                    }
                }
                return retVal;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetUserName(this IIdentity _identity)
        {
            string retVal = "";
            try
            {
                if (_identity != null && _identity.IsAuthenticated)
                {
                    var ci = _identity as ClaimsIdentity;
                    string _userId = ci?.FindFirst(ClaimTypes.Name)?.Value;
                    retVal = _userId;
                }

                return retVal;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Guid GetAppClientId(this IIdentity _identity)
        {
            Guid retVal = Guid.Empty;
            try
            {
                if (_identity != null && _identity.IsAuthenticated)
                {
                    var ci = _identity as ClaimsIdentity;
                    string _clientId = ci?.FindFirst(Tipos.ClaimTypes.AppId)?.Value;

                    if (!string.IsNullOrEmpty(_clientId))
                    {
                        retVal = Guid.Parse(_clientId);
                    }
                }
                return retVal;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_identity"></param>
        /// <param name="permisoSolicitado"></param>
        /// <returns></returns>
        public static bool IsAuthorized(this IIdentity _identity,string permisoSolicitado)
        {
            if (_identity != null && _identity.IsAuthenticated)
            {
                var ci = _identity as ClaimsIdentity;

                if (ci == null) return false;

                return ci.HasClaim(Tipos.ClaimTypes.Permiso, permisoSolicitado);
            }
            return false;
        }



        public static IReadOnlyList<string> GetRoles(this IIdentity _identity)
        {
            if (_identity != null && _identity.IsAuthenticated)
            {
                var ci = _identity as ClaimsIdentity;

                if (ci == null) return new List<string>();

                return ci.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            }
            return new List<string>();
        }
    }
}