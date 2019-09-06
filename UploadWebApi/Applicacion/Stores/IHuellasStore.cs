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
        Task<HuellaDto> ReadAsync(string idMuestra, Guid aplicacion);
        Task CreateAsync(HuellaDto huella,byte[] huellaRaw);
        void Delete(int idHuella);
        Task<byte[]> ReadHuellaRawAsync(int idHuella);
        Task WriteHuellaRawAsync(int idHuella, byte[] buffer);
    }
}
