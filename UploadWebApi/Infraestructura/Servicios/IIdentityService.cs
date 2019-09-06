using System;
using System.Collections.Generic;

namespace UploadWebApi.Infraestructura.Servicios
{

    public interface IIdentityService
    {
        bool IsSysAdmin { get; }

        bool IsAuthenticated { get; }

        Guid UserIdentity { get; }

        string UserName { get; }

        Guid AppIdentity { get; }

        bool IsAuthorized(string permisoSolicitado);

        IReadOnlyList<string> Roles { get; }

        //IList<string> Permisos { get; }

    }// IIdentityService
}