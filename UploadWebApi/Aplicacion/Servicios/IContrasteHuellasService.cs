/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:12:53
 *
 */

using System.Collections.Generic;
using System.Threading.Tasks;

using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContrasteHuellasService
    {


        Task<ContrasteDto> ConstrastarHuellasAsync(string idMuestra1, string idMuestra2);



        Task<ContrasteDto> ConstrastarHuellasAsync(IEnumerable<string> muetras);


    }
}