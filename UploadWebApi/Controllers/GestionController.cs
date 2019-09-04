using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using UploadWebApi.Models;

namespace UploadWebApi.Controllers
{
    [RoutePrefix("gestion")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GestionController : ApiController
    {


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

                await Task.CompletedTask;
                return Ok();
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

            var fileuploadPath = HttpContext.Current.Server.MapPath("~/App_Data");

            var md5 = CalcularMD5(dto.Stream);

            try
            {


                if (md5 == dto.Hash)
                {
                    using (FileStream file = new FileStream(Path.Combine(fileuploadPath, dto.NombreFichero), FileMode.CreateNew))
                    {

                        file.Write(dto.Stream, 0, dto.Stream.Length);

                    }
                    return await Task.FromResult(Created("/gestion/" + dto.IdMuestra, new GetHuellaDto { IdMuestra =dto.IdMuestra, FechaAnalisis=dto.FechaAnalisis }));
                }
                else
                    return await Task.FromResult(BadRequest("La verificación de firmas no es correcta."));

            }
            catch (IOException)
            {
                return await Task.FromResult(BadRequest($"El archivo {dto.NombreFichero} ya existe en el sistema."));
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


}
