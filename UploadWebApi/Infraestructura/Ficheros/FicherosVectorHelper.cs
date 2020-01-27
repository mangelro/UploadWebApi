/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 27/01/2020 11:42:03
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Infraestructura.Ficheros
{
    /// <summary>
    /// 
    /// </summary>
    public static class FicherosVectorHelper
    {

        /// <summary>
        /// Guarde un Stream como fichero en la ruta especificada
        /// </summary>
        /// <param name="rawHuella"></param>
        /// <param name="rutaFichero"></param>
        public static void GuardarFichero(Stream rawHuella, string rutaFichero)
        {
            byte[] buffer = new byte[512];

            int leidos = 0;
            using (FileStream file = new FileStream(rutaFichero, FileMode.CreateNew, FileAccess.Write))
            {
                while ((leidos = rawHuella.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, leidos);
                }
                file.Flush();
            }
        }

        /// <summary>
        /// Devuelve el nombre formateado correctamente del fichero 
        /// </summary>
        /// <param name="nombreFichero"></param>
        /// <param name="idHuella"></param>
        /// <returns></returns>
        public static string GetFormatoNombre(string nombreFichero, int idHuella)
        {
            string nombre = Path.GetFileNameWithoutExtension(nombreFichero);
            string extension = Path.GetExtension(nombreFichero);

            return $"{nombre}_{idHuella}_{extension}";
        }

        // <summary>
        /// Borra un fichero del sistema de archivos en la ruta establecida
        /// </summary>
        /// <param name="nombreFichero"></param>
        /// <returns></returns>
        public static bool BorrarFichero(string rutaFichero)
        {

            FileInfo file = new FileInfo(rutaFichero);

            if (file.Exists)
            {
                file.Delete();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Codifica un stream en un formato B64
        /// </summary>
        /// <param name="streamFile"></param>
        /// <returns></returns>
        public static string CodificarFichero(Stream streamFile)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                streamFile.CopyTo(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}