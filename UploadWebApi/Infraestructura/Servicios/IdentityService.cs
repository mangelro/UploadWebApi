using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

using Microsoft.Owin;

using UploadWebApi.Aplicacion.Servicios;
using UploadWebApi.Infraestructura.Extensiones;

namespace UploadWebApi.Infraestructura.Servicios
{
    public class IdentityService : IIdentityService
    {
        const string ROL_ADMINISTRADOR= "administradores";

        readonly IIdentity _identityManager;
        readonly IOwinContext _context;
        Guid _appIdentity;

        public IdentityService(IOwinContext context)
        {
            _identityManager = context?.Authentication.User.Identity;
            _context = context;
            _appIdentity = _identityManager.GetAppClientId();
        }

        /// <summary>
        /// Determina si el usuario posee credenciales de administrador
        /// </summary>
        public bool IsSysAdmin => Roles.Where(s => s.Equals(ROL_ADMINISTRADOR)).Any();

        /// <summary>
        /// 
        /// </summary>
        public bool IsAuthenticated => _identityManager.IsAuthenticated;

        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public Guid UserIdentity => _identityManager.GetUserId();

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string UserName => _identityManager.GetUserName();

        /// <summary>
        /// Identificador de la aplicación a través de la que se ha 
        /// identificado el usuario 
        /// </summary>
        public Guid AppIdentity => _appIdentity;

        /// <summary>
        /// Determina si el usuario 
        /// </summary>
        /// <param name="permiso"></param>
        /// <returns></returns>
        public bool IsAuthorized(string permiso) => _identityManager.IsAuthorized(permiso);

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> Roles => _identityManager.GetRoles();



    }// IdentityService
       

}