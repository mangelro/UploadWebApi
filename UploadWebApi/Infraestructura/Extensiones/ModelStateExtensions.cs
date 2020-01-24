/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 21/01/2020 16:01:18
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace UploadWebApi.Infraestructura.Extensiones
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelStateExtensions
    {

        /// <summary>
        /// Detorna un array con los mensajes de error del enlace del modelo
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string[] MensajesError(this ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(e => e.Errors).Select(e => e.Exception?.Message ?? e.ErrorMessage).ToArray();
        }
    }
}