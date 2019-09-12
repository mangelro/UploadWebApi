using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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

    }
}