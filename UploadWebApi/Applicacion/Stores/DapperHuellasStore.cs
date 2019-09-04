/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:48:45
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadWebApi.Models;

namespace UploadWebApi.Applicacion.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public class DapperHuellasStore : IHuellasStore
    {

        readonly IStoreConfiguration _config;

        public DapperHuellasStore(IStoreConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }


        public void Create(HuellaDto huella)
        {
            throw new NotImplementedException();
        }

        public void Delete(int idHuella)
        {
            throw new NotImplementedException();
        }

        public HuellaDto Read(string idMuestra, Guid aplicacion)
        {
            throw new NotImplementedException();
        }

        public Stream ReadStream(int idHuella)
        {
            throw new NotImplementedException();
        }

        public void WriteStream(int idHuella, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}