/*
 * Copyright ©2022 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera - prog2@oliva.net
 * Fecha: 04/02/2022 14:17:15
 *
 */

using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace UploadWebApi.Infraestructura.Binding
{

    /// <summary>
    /// Clase abstracta de IModelBinder que permite la ejecución de métodos asincronos
    /// </summary>
    public abstract class AsyncModelBinder : IModelBinder
    {
        protected abstract Task<bool> BindModelAsync(HttpActionContext actionContext, ModelBindingContext bindingContext);

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            return AsyncUtil.RunSync(() => BindModelAsync(actionContext, bindingContext));
        }
    }


}