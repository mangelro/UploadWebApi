/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:12:53
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FundacionOlivar.Web.Modelos;
using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGestionHuellasService
    {


        Task VerificarFichero(Stream rawFile, string hash);

        Task<Tuple<IEnumerable<GetRowHuellaDto>, int>> ConsultarHuellasAsync(RangoPaginacion paginacion, OrdenType orden);

        Task<GetHuellaDto> ConsultarHuellaAsync(string idMuestra);

        Task<GetHuellaDto> CrearRegistroHuellaAsync(InsertHuellaDto dto);

        Task BorrarRegistroHuellaAsync(string idMuestra);

        Task<BlobDto> DownloadHuellaAsync(string idMuestra);

        Task<BlobDto> DownloadHuellaAsync(int idHuella);

        Task<string> CalcularHash(Stream streamVector);
    }


}
