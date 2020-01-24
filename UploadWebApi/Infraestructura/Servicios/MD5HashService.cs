/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:31:11
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadWebApi.Applicacion;

namespace UploadWebApi.Infraestructura.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public class MD5HashService : IHashService
    {
        public byte[] CalcularHash(byte[] buffer)
        {

            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                return md5.ComputeHash(buffer);
            }
        }
            

        public bool VerifyHash(byte[] raw, string hashEsperado)
        {
            string md5Actual = Convert.ToBase64String(CalcularHash(raw));
            return hashEsperado.Equals(md5Actual, StringComparison.InvariantCulture);
        }
    }   
}