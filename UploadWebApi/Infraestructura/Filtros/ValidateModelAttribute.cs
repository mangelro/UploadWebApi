/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 19/09/2019 14:46:11
 *
 */

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Linq;

namespace UploadWebApi.Infraestructura.Filtros
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateModelAttribute: ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var error= actionContext.ModelState.Values.SelectMany(v => v.ToString()).ToString();

                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }

    }
}