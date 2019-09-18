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
    public class GetRowHuellaDto
    {
        public string IdMuestra { get; set; }
        public DateTime FechaAnalisis { get; set; }
        public string NombreFichero { get; set; }
    }
}