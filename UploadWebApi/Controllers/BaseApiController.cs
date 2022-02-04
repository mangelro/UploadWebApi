using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace UploadWebApi.Controllers
{
    public abstract class BaseApiController:ApiController
    {

        protected IHttpActionResult NoContent()
        {
            {
           
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.NoContent);
                return ResponseMessage(responseMsg);

            }
        }

        protected IHttpActionResult NotFound(string message)
        {
            {

                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent($"{{\"message\":\"{message}\"}}")
                };


                responseMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return ResponseMessage(responseMsg);

            }
        }



        /// <summary>
        /// Reemplaza el número de pagina en la url de paginación
        /// </summary>
        /// <param name="newNumPage"></param>
        /// <returns></returns>
        protected string GetLinkNewPage(int newNumPage)
        {

            if (newNumPage <= 0)
            {
                throw new ArgumentException("El número de página ha de ser mayor que cero.");
            }


            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Path);
            var query = Request.RequestUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);


            var newQuery = Regex.Replace(query, @"([?&]pageNumber)=[^?&]+", $"$1={newNumPage}");

            return baseUrl + "?" + newQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdHuella"></param>
        /// <returns></returns>
        protected string GetLinkDescarga(int IdHuella)
        {
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Path);
            return $"{baseUrl}/{IdHuella}/download";
        }


        protected string GetLinkDescarga(int IdHuella, string idMuestra)
        {
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            return $"{baseUrl}/v1/gestion/{idMuestra}/{IdHuella}/download";
        }

        protected string GetLinkDetalle(int IdHuella, string idMuestra)
        {
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            return $"{baseUrl}/v1/gestion/{idMuestra}";
        }

    }
}