using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Owin;
using UploadWebApi.Infraestructura.Extensiones;

namespace UploadWebApi.Infraestructura.Servicios.impl
{
    public class IdentityService : IIdentityService
    {

        readonly IIdentity _identityManager;
        readonly IOwinContext _context;
        Guid _appIdentity;

        public IdentityService(IOwinContext context)
        {
            _identityManager = context?.Authentication.User.Identity;
            _context = context;
            _appIdentity = _identityManager.GetAppClientId();
        }

        public bool IsSysAdmin => Roles.Where(s => s.Equals("administradores")).Any();

        public bool IsAuthenticated => _identityManager.IsAuthenticated;

        public Guid UserIdentity => _identityManager.GetUserId();

        public string UserName => _identityManager.GetUserName();

        public Guid AppIdentity => _appIdentity;

        public bool IsAuthorized(string permiso) => _identityManager.IsAuthorized(permiso);

        public IReadOnlyList<string> Roles => _identityManager.GetRoles();

    }// IdentityService
       

}