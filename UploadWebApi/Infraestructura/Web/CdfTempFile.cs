/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 11/09/2019 14:11:37
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FundacionOlivar.IO;

namespace UploadWebApi.Infraestructura.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class CdfTempFile : TemporalFile
    {
        public CdfTempFile(string tempPath):base(tempPath)
        {
        }

        protected override string GetTempFileName()
        {
            return $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}_TEMP.CDF";
        }
    }
}