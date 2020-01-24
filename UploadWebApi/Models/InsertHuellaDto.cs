/*
* Copyright © 2019 Fundación del Olivar
* Todos los derechos reservados
* Autor: Miguel A. Romera
* Fecha: 26/06/2019 10:15:16
*/

using System;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using UploadWebApi.Infraestructura.Serializacion;

namespace UploadWebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class InsertHuellaDto
    {

        [Required]
        public string IdMuestra { get; set; }

        [Required]
        public DateTime FechaAnalisis { get; set; }

        [Required]
        public string NombreFichero { get; set; }

        [Required]
        public string Hash { get; set; }

        [Required]
        [JsonConverter(typeof(Base64ArrayConverter))]
        public byte[] Stream { get; set; }


        public string Observaciones { get; set; }
    }
}