/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 30/01/2020 9:52:26
 *
 */

using System;
using System.Collections.Generic;


namespace UploadWebApi.Aplicacion.Stores
{
    public interface IQueryResult<out T>
         where T : class
    {
        IEnumerable<T> Items { get; }

        int TotalCount { get; }
    }
}