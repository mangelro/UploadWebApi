/*
 * Copyright ©2022 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera - prog2@oliva.net
 * Fecha: 04/02/2022 14:08:00
 *
 */

using System.IO;
using System.Web.Http.ModelBinding;

using UploadWebApi.Infraestructura.Binding;

namespace UploadWebApi.Infraestructura
{
    /// <summary>
    /// Interfaz para los fichero subidos 
    /// </summary>
    public interface IHttpPostedFile
    {
        string FileName { get; }

        string ContentType { get; }

        int ContentLength { get; }
        Stream InputStream { get; }
    }
}