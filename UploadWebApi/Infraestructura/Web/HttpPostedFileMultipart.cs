
/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 21/01/2020 15:38:20
 *
 */

using System.IO;
using System.Web;

namespace UploadWebApi.Infraestructura.Web
{
    /// <summary>
    /// Representa un fichero subido por un cliente via multipart/form-data 
    /// </summary>
    public class HttpPostedFileMultipart : HttpPostedFileBase
    {
        private readonly MemoryStream _fileContents;

        public override int ContentLength => (int)_fileContents.Length;
        public override string ContentType { get; }
        public override string FileName { get; }
        public override Stream InputStream => _fileContents;

        /// <summary>
        /// Inicializa una mueva instancia <see cref="HttpPostedFileMultipart"/> class. 
        /// </summary>
        /// <param name="fileName">Nombre completo cualificado del fichero del cliente</param>
        /// <param name="contentType">Contenido MIME type del fichero cargado</param>
        /// <param name="fileContents">Contenido RAW del fichero  cargado.</param>
        public HttpPostedFileMultipart(string fileName, string contentType, byte[] fileContents)
        {
            FileName = fileName;
            ContentType = contentType;
            _fileContents = new MemoryStream(fileContents);
        }
    }
}