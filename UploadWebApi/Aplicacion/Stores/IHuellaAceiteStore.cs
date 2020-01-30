using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FundacionOlivar.Modelos.ModelView;
using UploadWebApi.Aplicacion.Modelo;


namespace UploadWebApi.Aplicacion.Stores
{


    public interface IHuellaAceiteStore
    {

        Task<IQueryResult<HuellaAceite>> ReadAllAsync(RangoPaginacion paginacion, Guid idUsuario, Guid idAplicacion, OrdenType orden);

        Task<HuellaAceite> ReadAsync(string idMuestra, Guid idUsuario, Guid idAplicacion);

        Task<HuellaAceite> ReadAsync(int idHuella);

        Task CreateAsync(HuellaAceite huella,Stream huellaRaw);

        Task DeleteAsync(int idHuella);

        Task BloquearAsync(int idHuella, DateTime fechaBloqueo);


        Task<Stream> ReadHuellaRawAsync(int idHuella);

        Task WriteHuellaRawAsync(int idHuella, Stream buffer);
    }
}
