/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 30/01/2020 9:42:59
 *
 */

using System.Collections.Generic;
using UploadWebApi.Aplicacion.Stores;

namespace UploadWebApi.Infraestructura.Datos
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryResult<T> : IQueryResult<T> where T : class
    {
        public QueryResult(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }

        public IEnumerable<T> Items { get; }

        public int TotalCount { get; }
    }
}