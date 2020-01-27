/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 * Autor: Miguel A. Romera
 * Fecha: 26/06/2019 10:15:16
 */

using System;
using Newtonsoft.Json;

namespace UploadWebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class GetHuellaDto
    {
        [JsonIgnore()]
        public int IdHuella { get; set; }
        //
        public string IdMuestra { get; set; }
        public DateTime FechaAnalisis { get; set; }
        public string NombreFichero { get; set; }
        public string Hash { get; set; }
        public Guid IdRegistrador { get; set; }
        public string NombreRegistrador { get; set; }
        public bool VectorReferencia { get; set; }
        public string Observaciones { get; set; }
        //
        public string LinkDescarga { get; set; }
    }
}