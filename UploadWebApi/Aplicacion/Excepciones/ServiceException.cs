/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:16:56
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Aplicacion.Excepciones
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceException: Exception
    {

        public ServiceException(string message) : base(message)
        { }

        public ServiceException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}