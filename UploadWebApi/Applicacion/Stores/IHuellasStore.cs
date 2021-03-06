using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadWebApi.Models;

namespace UploadWebApi.Applicacion.Stores
{
    public enum OrdenListatoTipo
    {
        ASC,
        DESC,
    }

    public interface IHuellasStore
    {

        Task<Tuple<IEnumerable<HuellaDto>, int>> ReadAllAsync(int pageNumber, int pageSize, Guid idUsuario, Guid idAplicacion, OrdenListatoTipo orden = OrdenListatoTipo.DESC);
        Task<HuellaDto> ReadAsync(string idMuestra, Guid idUsuario, Guid idAplicacion);
        Task<HuellaDto> ReadAsync(int idHuella);

        Task CreateAsync(HuellaDto huella,byte[] huellaRaw);
        Task DeleteAsync(int idHuella);
        Task BloquearAsync(int idHuella, DateTime fechaBloqueo);


        Task<byte[]> ReadHuellaRawAsync(int idHuella);
        Task WriteHuellaRawAsync(int idHuella, byte[] buffer);
    }
}
