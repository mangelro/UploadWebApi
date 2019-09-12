/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 10/09/2019 17:53:03
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ContrasteDto
    {
        public double IndiceSimilitud { get; set; }
        public string Estado { get; set; }
    }
}