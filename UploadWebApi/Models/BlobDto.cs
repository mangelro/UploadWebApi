/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 05/09/2019 10:24:48
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BlobDto
    {
        public Stream Raw { get; set; }
        public string Hash { get; set; }
        public string NombreFichero { get; set;}

    }
}