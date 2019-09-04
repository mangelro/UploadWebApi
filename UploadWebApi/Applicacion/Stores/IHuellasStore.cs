using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadWebApi.Models;

namespace UploadWebApi.Applicacion.Stores
{
    public interface IHuellasStore
    {
        HuellaDto Read(string idMuestra, Guid aplicacion);
        void Create(HuellaDto huella);
        void Delete(int idHuella);
        Stream ReadStream(int idHuella);
        void WriteStream(int idHuella, Stream stream);
    }
}
