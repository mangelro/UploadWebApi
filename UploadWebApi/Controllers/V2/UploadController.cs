using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;

using UploadWebApi.Aplicacion.Servicios;
using UploadWebApi.Infraestructura;
using UploadWebApi.Infraestructura.Binding;

namespace UploadWebApi.Controllers.V2
{

    /// <summary>
    /// hola
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
        //public IHttpActionResult Post(UploadMuestraV2 upload)
        //public IHttpActionResult Post([ModelBinder(typeof(UploadedFilesModelBinder))] IEnumerable<IHttpPostedFile> files)
        //public IHttpActionResult Post([ModelBinder(typeof(UploadedFilesModelBinder))] IHttpPostedFile files)
        public IHttpActionResult Post([ModelBinder] IList<IHttpPostedFile> files)
        {
            try
            {
                //files.SaveAs(System.IO.Path.Combine(@"C:\uploadfiles", files.FileName));
                return Ok();

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }


            //if (!ModelState.IsValid)
            //{
            //    var errorModel = ModelState.MensajesError();
            //    var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, String.Join(",", errorModel.ToArray()));
            //    return ResponseMessage(response);
            //}
            //if (!_hashService.VerifyHash(upload.VectorDatos.InputStream, upload.Hash))
            //{
            //    var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "La validación HASH es errónea");
            //    return ResponseMessage(response);
            //}
            //return Content(HttpStatusCode.OK, new
            //{
            //    Status = "success",
            //    upload.IdMuestra,
            //    upload.FechaAnalisis,
            //    upload.VectorDatos.FileName,
            //    upload.VectorDatos.ContentLength,
            //    upload.VectorDatos.ContentType
            //});
        }

    }



}
