/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 21/01/2020 17:38:05
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Infraestructura.Extensiones
{
    /// <summary>
    /// 
    /// </summary>
    public static class StreamExtensions
    {

        /// <summary>
        /// Devuelve un array de bytes a partir de un Stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToArray(this Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}
