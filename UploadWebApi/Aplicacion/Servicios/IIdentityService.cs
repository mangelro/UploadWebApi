using System;
using System.Collections.Generic;

namespace UploadWebApi.Aplicacion.Servicios
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

    }// IIdentityService
}