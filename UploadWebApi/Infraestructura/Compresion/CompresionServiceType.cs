/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 16/09/2019 11:36:34
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Infraestructura.Compresion
{
    /// <summary>
    /// 
    /// </summary>
    public enum CompresionServiceType
    {
        FakeCompresionService,
        GZipCompresionService,
        SevenZipCompresionService
    }
}