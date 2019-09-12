/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 10/09/2019 14:22:57
 *
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UploadWebApi.Infraestructura.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueryStringHelper
    {
        public static NameValueCollection ParseQuery(string queryString)
        {
            var result = ParseNullableQuery(queryString);

            if (result == null)
            {
                return new NameValueCollection();
            }

            return result;
        }
               
        public static NameValueCollection ParseNullableQuery(string queryString)
        {


            if (string.IsNullOrEmpty(queryString) || queryString == "?")
            {
                return null;
            }

            return HttpUtility.ParseQueryString(queryString);
        }
    }
}