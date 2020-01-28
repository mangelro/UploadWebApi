/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 28/01/2020 13:16:45
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Infraestructura.Extensiones
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Verifica si existen identificadores duplicados en la coleccion
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="exceptionMessage">Mensaje de la excepcion debe incluir {0} pa</param>
        /// <exception cref="ArgumentException">Existe un Identificador duplicado</exception>
        public static void VerificarDuplicidadId(this IEnumerable<string> ids,string exceptionMessage="")
        {
            HashSet<string> identificadores = new HashSet<string>();
            foreach (var id in ids)
                if (!identificadores.Add(id))
                {
                    if (String.IsNullOrEmpty(exceptionMessage))
                        throw new ArgumentException($"El identificador '{id}' se encuentra duplicado.");
                    else
                        throw new ArgumentException(String.Format(exceptionMessage,id));
                }
        }

    }
}