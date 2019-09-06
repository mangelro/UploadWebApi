using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using UploadWebApi.Applicacion.Servicios;
using UploadWebApi.Infraestructura.Servicios;
using UploadWebApi.Models;

namespace UploadWebApi.Controllers.V1
{

    [Authorize]
    [RoutePrefix("v1/gestion")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GestionController : ApiController
    {

        readonly GestionHuellasService _service;
        readonly IIdentityService _identity;

        public GestionController(GestionHuellasService service, IIdentityService identity)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _identity= identity ?? throw new ArgumentNullException(nameof(identity));

        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            try
            {

                await Task.CompletedTask;
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{idMuestra}")]
        public async Task<IHttpActionResult> Get(string idMuestra)
        {
            try
            {
                var huella = await _service.ConsultarHuellaAsync(idMuestra, _identity.AppIdentity);

                return Ok(huella);
            }
            catch (NotFoundException noEx)
            {
                return BadRequest(noEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("{idMuestra}/download")]
        public async Task<IHttpActionResult> Descarga(string idMuestra)
        {
            try
            {
                BlobDto dataStream = await _service.DownloadHuellaAsync(idMuestra, _identity.AppIdentity);



                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(dataStream.Raw)
                };
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = dataStream.NombreFichero
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                //HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                //{
                //    Content = new StreamContent(dataStream.Raw)
                //    {
                //        Headers = {
                //            ContentType = new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Applicat‌​ion.Octet),
                //            ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = dataStream.NombreFichero}
                //        }
                //    }
                //};

                return ResponseMessage(response);
            }
            catch (NotFoundException noEx)
            {
                return BadRequest(noEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(InsertHuellaDto dto)
        {
            try
            {
                var inserted = await _service.CrearRegistroHuellaAsync(dto, _identity.UserIdentity, _identity.AppIdentity);

                return Created("v1/gestion/" + inserted.IdMuestra, inserted);
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpPost]
        [Route("{idMuestra}")]
        public async Task<IHttpActionResult> Delete(string idMuestra)
        {
       
            try
            {

                await Task.CompletedTask;
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }












    }






    //static class MultipartFormDataStreamProviderExtensions
    //{

    //    public static void Rename(this MultipartFormDataStreamProvider provider)
    //    {

    //        var uploadingFilesNames = provider.FileData.Select(x => x.LocalFileName).ToList();
    //        var originalFilesNames = provider.Contents.Select(c => c.Headers.ContentDisposition.FileName.Trim(new Char[] { '"' })).ToList();


    //        if (uploadingFilesNames.Count != originalFilesNames.Count)
    //            throw new ArgumentException("");


    //        for (var i=0; i< uploadingFilesNames.Count; i++)
    //        {
    //            var upFileName = uploadingFilesNames[0];

    //            var pathUpFileName = System.IO.Path.GetDirectoryName(upFileName);

    //            var newFileName = System.IO.Path.Combine(pathUpFileName, String.Concat(originalFilesNames[0]));

    //            int contCopias = 1;

    //            var orgNewFilename = originalFilesNames[0];
    //            while (System.IO.File.Exists(newFileName))
    //            {
    //                newFileName = System.IO.Path.Combine(pathUpFileName, $"{System.IO.Path.GetFileNameWithoutExtension(orgNewFilename)}_copia({contCopias}){System.IO.Path.GetExtension(orgNewFilename)}");
    //                contCopias++;
    //            }

    //            System.IO.File.Move(upFileName, newFileName);
    //        }


    //    }

    //}

    /// <summary>
    /// Permite subir fichero y acceder en formato binario
    /// </summary>
    /// <returns></returns>
    /*
       public async Task<IHttpActionResult> PostUpload()
       {
           if (!Request.Content.IsMimeMultipartContent())
           {
               return StatusCode(HttpStatusCode.UnsupportedMediaType);
           }

           var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();


           string md5=string.Empty;
           foreach (var stream in filesReadToProvider.Contents)
           {
               var fileBytes = await stream.ReadAsByteArrayAsync();
               md5 = CalcularMD5(fileBytes);
           }

           return Ok(md5);
       }


       [Route("")]
       [HttpPost]
       public async Task<IHttpActionResult> PostUpload()
       {
           try
           {
               ///var fileuploadPath = ConfigurationManager.AppSettings["FileUploadLocation"];

               if (!Request.Content.IsMimeMultipartContent())
               {
                   return StatusCode(HttpStatusCode.UnsupportedMediaType);
               }

               var fileuploadPath = HttpContext.Current.Server.MapPath("~/App_Data");

               var provider = new MultipartFormDataStreamProvider(fileuploadPath);

               var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));

               foreach (var header in Request.Content.Headers)
               {
                   content.Headers.TryAddWithoutValidation(header.Key, header.Value);
               }

               await content.ReadAsMultipartAsync(provider);

               provider.Rename();

               return Ok();
           }
           catch (Exception ex)
           {
               return BadRequest(ex.Message);
           }

       }
       */

}
