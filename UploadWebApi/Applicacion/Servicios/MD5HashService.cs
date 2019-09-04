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

namespace UploadWebApi.Applicacion.Servicios
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
    }
}