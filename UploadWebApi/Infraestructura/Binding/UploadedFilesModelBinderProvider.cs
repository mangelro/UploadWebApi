/*
 * Copyright ©2022 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera - prog2@oliva.net
 * Fecha: 04/02/2022 14:19:19
 *
 */

using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace UploadWebApi.Infraestructura.Binding
{
    public class UploadedFilesModelBinderProvider : ModelBinderProvider
    {
        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (modelType== typeof(IHttpPostedFile)|| modelType.IsAssignableFrom(typeof(IEnumerable<IHttpPostedFile>)))
            {
                return new UploadedFilesModelBinder();
            }

            return null;
        }
    }

}