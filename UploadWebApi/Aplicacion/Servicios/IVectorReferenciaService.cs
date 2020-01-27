/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 27/01/2020 10:08:57
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVectorReferenciaService
    {

        Task<GetHuellaDto> CrearRegistroHuellaRefenciaAsync(InsertVectorReferencia dto);

    }
}