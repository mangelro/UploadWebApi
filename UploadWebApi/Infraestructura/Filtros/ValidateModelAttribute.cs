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
using System.IO;

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
                var excepciones = actionContext.ModelState.Values.SelectMany(v => v.Errors).Select(e=>e.Exception).Where(e=>e!=null);
                var errores = actionContext.ModelState.Values.SelectMany(e => e.Errors).Select(v => v.ErrorMessage);

                if (excepciones.Any())
                {
                    using (var w = new StringWriter())
                    {
                        Newtonsoft.Json.JsonSerializer.Create().Serialize(w, excepciones.Select(m => m.Message).ToArray());
                        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, w.ToString());
                    }

                }
                else if (errores.Any())
                {
                    using (var w = new StringWriter())
                    {
                        Newtonsoft.Json.JsonSerializer.Create().Serialize(w, errores.ToArray());
                        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, w.ToString());
                    }
                }



            }
        }

    }
}