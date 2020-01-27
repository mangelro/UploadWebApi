/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 * Autor: Miguel A. Romera
 * Fecha: 26/06/2019 10:15:16
 */

using System;

namespace UploadWebApi.Aplicacion.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    public class HuellaAceite
    {
        public int IdHuella { get; set; }

        public string IdMuestra { get; set; }

        public DateTime FechaAnalisis { get; set; }

        public string NombreFichero { get; set; }

        public string Hash { get; set; }

        public Guid AppCliente { get; set; }

        public Guid Propietario { get; set; }

        public string NombrePropietario { get; set; }

        public DateTime? FechaBloqueo { get; set; }

        /// <summary>
        /// Determina si el vector es un  vector referencia
        /// formado por la operacion con otros vectores
        /// </summary>
        public bool VectorReferencia { get; set; }

        public string Observaciones { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool EstaBloqueada => FechaBloqueo.HasValue;
    }
}