/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/10/2019 10:50:43
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Applicacion.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public class MuestraDuplicadaException:Exception
    {
        public MuestraDuplicadaException(string message):base(message)
        {

        }

    }
}