/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 28/01/2020 11:56:24
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuellaDactilarAceite.IndicesSimilitud;
using UploadWebApi.Aplicacion.Servicios;
using UploadWebApi.Aplicacion.Servicios.Procesadores;
using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Validadores
{
    /// <summary>
    /// 
    /// </summary>
    public interface IValidadorContraste
    {

        /// <summary>
        /// Indice utilziado para la validacion
        /// </summary>
        IIndiceSimilitud Indice { get; }

        bool ValidarConstraste(IEnumerable<ItemIndice> indices);

    }
}