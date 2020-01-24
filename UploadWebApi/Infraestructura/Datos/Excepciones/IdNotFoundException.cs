/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:16:56
 *
 */

using System;


namespace UploadWebApi.Infraestructura.Datos.Excepciones
{
    /// <summary>
    /// Excepción que se produce cuando no existe la entidad en el sistema
    /// </summary>
    public class IdNoEncontradaException : Exception
    {

        public IdNoEncontradaException(string message) : base(message)
        { }

        public IdNoEncontradaException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}