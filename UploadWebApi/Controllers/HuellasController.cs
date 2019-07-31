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
    [RoutePrefix("huellas")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HuellasController : ApiController
    {


        /// <summary>
        /// Permite subir fichero y acceder en formato binario
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
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
        /*

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


        [Route("dto")]
        [HttpPost]
        public async Task<IHttpActionResult> PostDto(UploadInfoDto dto)
        {

            var fileuploadPath = HttpContext.Current.Server.MapPath("~/App_Data");

            var md5 = CalcularMD5(dto.Stream);

            try
            {


                if (md5 == dto.Hash)
                {
                    using (FileStream file = new FileStream(Path.Combine(fileuploadPath,dto.NombreFichero), FileMode.Create))
                    {

                        file.Write(dto.Stream, 0, dto.Stream.Length);

                    }

                    return await Task.FromResult(Ok(new { status = "HOLA" }));
                }
                else
                    return await Task.FromResult(BadRequest());

            }catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




        string CalcularMD5(byte[] buffer)
        {
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                return Convert.ToBase64String(md5.ComputeHash(buffer));
            }
        }


        string CalcularMD5(string buffer)
        {
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                return Convert.ToBase64String(md5.ComputeHash(Convert.FromBase64String(buffer)));
            }
        }

    }






    static class MultipartFormDataStreamProviderExtensions
    {

        public static void Rename(this MultipartFormDataStreamProvider provider)
        {

            var uploadingFilesNames = provider.FileData.Select(x => x.LocalFileName).ToList();
            var originalFilesNames = provider.Contents.Select(c => c.Headers.ContentDisposition.FileName.Trim(new Char[] { '"' })).ToList();


            if (uploadingFilesNames.Count != originalFilesNames.Count)
                throw new ArgumentException("");


            for (var i=0; i< uploadingFilesNames.Count; i++)
            {
                var upFileName = uploadingFilesNames[0];

                var pathUpFileName = System.IO.Path.GetDirectoryName(upFileName);

                var newFileName = System.IO.Path.Combine(pathUpFileName, String.Concat(originalFilesNames[0]));

                int contCopias = 1;

                var orgNewFilename = originalFilesNames[0];
                while (System.IO.File.Exists(newFileName))
                {
                    newFileName = System.IO.Path.Combine(pathUpFileName, $"{System.IO.Path.GetFileNameWithoutExtension(orgNewFilename)}_copia({contCopias}){System.IO.Path.GetExtension(orgNewFilename)}");
                    contCopias++;
                }

                System.IO.File.Move(upFileName, newFileName);
            }


        }

    }


}
