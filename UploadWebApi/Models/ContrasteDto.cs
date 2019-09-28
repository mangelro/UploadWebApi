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


    public enum EstadoContrasteType
    {
        VALIDO,
        INVALIDO
    }

    /// <summary>
    /// 
    /// </summary>
    public class ContrasteDto
    {
        public string IdMuestra1 { get; set; }
        public string IdMuestra2 { get; set; }
        public DateTime FechaContraste { get; set; }
        public double IndiceSimilitud { get; set; }
        public double UmbralAceptacion { get; set; }
        public EstadoContrasteType Estado { get; set; }
    }
}