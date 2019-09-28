using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace UploadWebApi.Controllers.V1
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


    }
}