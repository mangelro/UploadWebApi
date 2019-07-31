/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 * Autor: Miguel A. Romera
 * Fecha: 26/06/2019 10:15:16
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UploadWebApi.Infraestructura.Serializacion;

namespace UploadWebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadInfoDto
    {
        public string NombreFichero { get; set; }
        public DateTime FechaFichero { get; set; }
        public string Hash { get; set; }

        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] Stream { get; set; }
        public string ContentType { get; set; }
    }
}