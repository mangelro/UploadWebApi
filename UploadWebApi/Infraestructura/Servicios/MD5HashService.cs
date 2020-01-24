/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:31:11
 *
 */

using System;
using System.IO;

using UploadWebApi.Aplicacion.Servicios;

namespace UploadWebApi.Infraestructura.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public class MD5HashService : IHashService
    {

        public byte[] CalcularHash(Stream file)
        {

            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {

                byte[] buffer= md5.ComputeHash(file);

                file.Position = 0;
                return buffer;
            }
        }
           

        public bool VerifyHash(Stream file, string hashEsperado)
        {

            string md5Actual = Convert.ToBase64String(CalcularHash(file));
            return hashEsperado.Equals(md5Actual, StringComparison.InvariantCulture);
        }
    }   
}