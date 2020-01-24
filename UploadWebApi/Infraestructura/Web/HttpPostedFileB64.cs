
/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 21/01/2020 15:38:20
 *
 */

using System;
using System.IO;
using System.Web;

namespace UploadWebApi.Infraestructura.Web
{
    /// <summary>
    /// Representa un fichero subido por un cliente en formato b64
    /// </summary>
    public class HttpPostedFileB64 : HttpPostedFileBase
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
        public HttpPostedFileB64(string fileName, string contentType, string fileContents)
        {
            FileName = fileName;
            ContentType = contentType;
            _fileContents = new MemoryStream(ConvetToByteArray(fileContents));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private byte[] ConvetToByteArray(string content)
        {
            return Convert.FromBase64String(content);
        }

    }

}