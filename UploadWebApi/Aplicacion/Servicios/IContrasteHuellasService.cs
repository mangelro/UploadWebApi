/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:12:53
 *
 */

using System.Threading.Tasks;

using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContrasteHuellasService
    {
        Task<ContrasteDto> ConstrastarHuellas(string idMuestra1, string idMuestra2);
    }
}