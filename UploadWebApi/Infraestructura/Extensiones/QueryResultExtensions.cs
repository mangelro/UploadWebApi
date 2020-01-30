/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 30/01/2020 9:56:41
 *
 */

using System;
using System.Linq;
using FundacionOlivar.Modelos.ModelView;
using UploadWebApi.Aplicacion.Stores;

namespace UploadWebApi.Infraestructura.Extensiones
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueryResultExtensions
    {
        public static PaginatedList<T> AsPaginated<T>(this IQueryResult<T> result, int pageIndex, int pageSize, Func<int, string> newPageLink, Func<T,T> transformacion) where T : class
        {
                                  
           var paginacion =new PaginatedList<T>(result.Items.Select(transformacion), pageIndex, pageSize, result.TotalCount);

            //Link de pagina anterior
            if (paginacion.HasPreviousPage)
                paginacion.Links.PreviousPage = newPageLink(pageIndex - 1);


            //Link de pagina siguiente
            if (paginacion.HasNextPage)
                paginacion.Links.NextPage = newPageLink(pageIndex + 1);

            return paginacion;


        }

    }
}