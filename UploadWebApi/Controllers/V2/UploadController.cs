using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

using UploadWebApi.Aplicacion.Servicios;
using UploadWebApi.Infraestructura.Extensiones;
using UploadWebApi.Models;

namespace UploadWebApi.Controllers.V2
{

    /// <summary>
    /// </summary>
    [RoutePrefix("v2/upload")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UploadController : BaseApiController
    {


        readonly IHashService _hashService;

        public UploadController(IHashService hashService)
        {
            _hashService = hashService;
        }

        [Route("")]
        public IHttpActionResult Post(UploadMuestraV2 upload)
        {

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.MensajesError();
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, String.Join(",", errorModel.ToArray()));
                return ResponseMessage(response);
            }

            if (!_hashService.VerifyHash(upload.VectorDatos.InputStream, upload.Hash))
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "La validación HASH es errónea");
                return ResponseMessage(response);
            }


            return Content(HttpStatusCode.OK, new
            {
                Status = "success",
                upload.IdMuestra,
                upload.FechaAnalisis,
                upload.VectorDatos.FileName,
                upload.VectorDatos.ContentLength,
                upload.VectorDatos.ContentType
            });
        }

    }


    
}
