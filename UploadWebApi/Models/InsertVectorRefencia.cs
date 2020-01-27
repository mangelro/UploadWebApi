/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 27/01/2020 10:04:08
 *
 */

using System.Collections.Generic;


namespace UploadWebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class InsertVectorReferencia
    {

        public string IdMuestraReferencia { get; set; }

        /// <summary>
        /// Listado de las muestras que se van a  utilizar
        /// para obtener el vector referencia
        /// </summary>
        public IEnumerable<string> IdMuestras { get; set; }

    }
}