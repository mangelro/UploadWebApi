using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

using UploadWebApi.Infraestructura.Extensiones;

namespace UploadWebApi.Infraestructura.Binding
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadedFilesModelBinder : AsyncModelBinder
    {
        protected async override Task<bool> BindModelAsync(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            HttpRequestMessage request = actionContext.Request;

            if (!request.Content.IsMimeMultipartContent())
                return false;

            try
            {
                List<UploadFile> files = await EnlazarFicheros(request, bindingContext.ModelName);

                if (bindingContext.ModelType.IsAssignableFrom(typeof(IEnumerable<IHttpPostedFile>)))
                {
                    bindingContext.Model = files;
                }
                else if (files.Count > 0)
                {
                    bindingContext.Model = files[0];
                }
                return true;

            }
            catch
            {
                return false;
            }
        }

        private async Task<List<UploadFile>> EnlazarFicheros(HttpRequestMessage request, string modelName)
        {
            var provider = new MultipartMemoryStreamProvider();

            MultipartMemoryStreamProvider reader = await request.Content.ReadAsMultipartAsync(provider);

            IEnumerable<HttpContent> filesContents = reader.Contents.Where(c => c.Headers.ContentDisposition.Name.Trim('\"') == modelName).ToList();

            List<UploadFile> files = new List<UploadFile>();

            foreach (HttpContent file in filesContents)
            {
                byte[] buffer = await file.ReadAsByteArrayAsync();

                string fileName =file.Headers.ContentDisposition.FileName.Trim('\"');
                string contentType= file.Headers.ContentType.ToString();
                int contentLength= (int)file.Headers.ContentLength;

                files.Add(new UploadFile(fileName,contentType,contentLength,buffer));
            }

            return files;
        }





        /*
         * 
         * 
         */
        class UploadFile : IHttpPostedFile
        {
            private UploadFile() { }

            private readonly string _fileName;
            private readonly string _contentType;
            private readonly int _contentLength;
            private readonly byte[] _buffer;

            public UploadFile(string fileName, string contentType, int contentLength, byte[] buffer)
            {
                _fileName = fileName;
                _contentType = contentType;
                _contentLength = contentLength;
                _buffer = buffer;
            }
            public string FileName => _fileName;

            public string ContentType => _contentType;

            public int ContentLength => _contentLength;

            public Stream InputStream => new MemoryStream(_buffer, false);

            public void SaveAs(string filename)
            {
                InputStream.SaveToFile(filename);
            }
        }


    }


}
