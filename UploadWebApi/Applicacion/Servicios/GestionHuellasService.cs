/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:12:53
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using UploadWebApi.Applicacion.Mapeado;
using UploadWebApi.Applicacion.Stores;
using UploadWebApi.Infraestructura.netCDF;
using UploadWebApi.Infraestructura.Web;
using UploadWebApi.Models;

namespace UploadWebApi.Applicacion.Servicios
{




    /// <summary>
    /// 
    /// </summary>
    public class GestionHuellasService
    {

        readonly IConfiguracionRegistros _conf;
        readonly IHuellasStore _store;
        readonly IHashService _hashService;
        readonly IMapperService _mapperService;

        public GestionHuellasService(IConfiguracionRegistros config, IHuellasStore store, IHashService hashService, IMapperService mapperService)
        {
            _conf = config ?? throw new ArgumentNullException(nameof(config));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService));
            _mapperService = mapperService ?? throw new ArgumentNullException(nameof(mapperService));
        }

        /// <summary>
        /// Verifica la validez (firma y formato) del fichero
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Task VerificarFicheroCDF(byte[] raw, string hash)
        {

            using (var tempFile = new CdfTempFile(_conf.RutaTemporal))
            {
                if (!_hashService.VerifyHash(hash, raw))
                    throw new ServiceException($"La verificación de firmas de la huella no es correcta.");

                tempFile.Create(raw);

                var parser = new NetCDFParser(new NetCDFConfig());

                try
                {
                    parser.Procesar(tempFile.TempFileName);
                }
                catch (NetCDFException ex)
                {
                    throw new ServiceException(ex.Message);
                }

                return Task.CompletedTask;
            }
        }

        public async Task<Tuple<IEnumerable<GetRowHuellaDto>, int>> ConsultarHuellasAsync(int pageNumber, int pageSize, Guid idAplicacion, string orden)
        {


            return await ConsultarHuellasAsync(pageNumber, pageSize, Guid.Empty, idAplicacion, orden);
        }

        public async Task<Tuple<IEnumerable<GetRowHuellaDto>, int>> ConsultarHuellasAsync(int pageNumber, int pageSize, Guid idUsuario, Guid idAplicacion, string orden)
        {

            try
            {

                OrdenListatoTipo tipoOrden = (OrdenListatoTipo)Enum.Parse(typeof(OrdenListatoTipo), orden.ToUpper());

                var tupla = await _store.ReadAllAsync(pageNumber, pageSize, idUsuario, idAplicacion, tipoOrden);

                return Tuple.Create<IEnumerable<GetRowHuellaDto>, int>(_mapperService.Map<HuellaDto, GetRowHuellaDto>(tupla.Item1), tupla.Item2);

            }
            catch (ArgumentException ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }

        public async Task<GetHuellaDto> ConsultarHuellaAsync(string idMuestra, Guid idAplicacion)
        {
            return await ConsultarHuellaAsync(idMuestra, Guid.Empty, idAplicacion);
        }

        public async Task<GetHuellaDto> ConsultarHuellaAsync(string idMuestra, Guid idUsuario, Guid idAplicacion)
        {
            var huella = await _store.ReadAsync(idMuestra, idUsuario, idAplicacion);

            ThrowIfNull(idMuestra, huella);


            var dto = _mapperService.Map<HuellaDto, GetHuellaDto>(huella);

            //dto.LinkDescarga = $"/{huella.IdHuella}/download";

            return dto;
        }

        public async Task<GetHuellaDto> CrearRegistroHuellaAsync(InsertHuellaDto dto, Guid idUsuario, Guid idAplicacion)
        {
            try
            {
                HuellaDto inserted = null;
                using (TransactionScope tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {


                    inserted = new HuellaDto
                    {
                        IdMuestra = dto.IdMuestra,
                        NombreFichero = dto.NombreFichero,
                        FechaAnalisis = dto.FechaAnalisis,
                        AppCliente = idAplicacion,
                        Propietario = idUsuario,
                        Hash = dto.Hash,
                    };

                    await _store.CreateAsync(inserted, dto.Stream);

                    var fileuploadPath = _conf.RutaFicheros;

                    CrearFichero(dto.Stream, Path.Combine(fileuploadPath, GetFormatoNombre(dto.NombreFichero, inserted.IdHuella)));
                    
                    tran.Complete();

                    return _mapperService.Map<HuellaDto, GetHuellaDto>(inserted);

                }//using trans
            }
            catch (IOException)
            {
                throw new ServiceException($"El archivo {dto.NombreFichero} ya existe en el sistema.");
            }
            catch (TransactionAbortedException ex)
            {
                throw new ServiceException(ex.Message);
            }

        }

        public async Task BorrarRegistroHuellaAsync(string idMuestra, Guid idUSuario, Guid idAplicacion, bool forzarBorrado)
        {

            HuellaDto huella = null;

            try
            {
                using (TransactionScope tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    huella = await _store.ReadAsync(idMuestra, idUSuario, idAplicacion);

                    ThrowIfNull(idMuestra, huella);


                    if (huella.EstaBloqueada && !forzarBorrado)
                    {
                        throw new ServiceException($"La huella {idMuestra} está bloqueada y no puede ser eliminada.");
                    }

                    await _store.DeleteAsync(huella.IdHuella);

                    var fileuploadPath = _conf.RutaFicheros;

                    BorrarFichero(Path.Combine(fileuploadPath, GetFormatoNombre(huella.NombreFichero, huella.IdHuella)));

                    tran.Complete();
                }

            }
            catch (IOException)
            {
                throw new ServiceException($"El archivo {huella.NombreFichero} no existe en el sistema.");
            }
            catch (TransactionAbortedException ex)
            {
                throw new ServiceException(ex.Message);
            }

        }

        public async Task<BlobDto> DownloadHuellaAsync(string idMuestra, Guid idApplicacion)
        {
            var huella = await _store.ReadAsync(idMuestra, Guid.Empty, idApplicacion);

            ThrowIfNull(idMuestra, huella);

            return await DownloadHuellaInternalAsync(huella);
        }

        public async Task<BlobDto> DownloadHuellaAsync(int idHuella)
        {

            var huella = await _store.ReadAsync(idHuella);

            ThrowIfNull("", huella);

            return await DownloadHuellaInternalAsync(huella);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="huella"></param>
        /// <returns></returns>
        async Task<BlobDto> DownloadHuellaInternalAsync(HuellaDto huella)
        {

            byte[] huellaRaw = await _store.ReadHuellaRawAsync(huella.IdHuella);

            if (!_hashService.VerifyHash(huella.Hash, huellaRaw))
                throw new ServiceException($"La verificación de firmas de la muestra {huella.IdMuestra} no es correcta.");


            return new BlobDto
            {
                Raw = new MemoryStream(huellaRaw),
                NombreFichero = huella.NombreFichero,
                Hash = huella.Hash,
            };
        }
        
        void ThrowIfNull(string idMuestra, HuellaDto huella)
        {
            if (huella == null)
                throw new NotFoundException($"La huella de la muestra {idMuestra} no existe en el sistema o no está autorizado.");
        }

        static string GetFormatoNombre(string nombreFichero, int idHuella)
        {
            string nombre = Path.GetFileNameWithoutExtension(nombreFichero);
            string extension = Path.GetExtension(nombreFichero);

            return $"{nombre}_{idHuella}_{extension}";
        }

        /// <summary>
        /// Crea un fichero en el sistema de archivos en la ruta establecida
        /// </summary>
        /// <param name="rawHuella"></param>
        /// <param name="rutaFichero"></param>
        static void CrearFichero(byte[] rawHuella, string rutaFichero)
        {
            byte[] buffer = new byte[255];

            int leidos = 0;
            using (FileStream file = new FileStream(rutaFichero, FileMode.CreateNew, FileAccess.Write))
            {
                using (var stream = new MemoryStream(rawHuella, false))
                {
                    do
                    {
                        leidos = stream.Read(buffer, 0, buffer.Length);
                        file.Write(buffer, 0, leidos);
                    } while (leidos == buffer.Length);
                }
            }
        }
        /// <summary>
        /// Borra un fichero del sistema de archivos en la ruta establecida
        /// </summary>
        /// <param name="nombreFichero"></param>
        /// <returns></returns>
        static bool BorrarFichero(string rutaFichero)
        {

            FileInfo file = new FileInfo(rutaFichero);

            if (file.Exists)
            {
                file.Delete();
                return true;
            }
            else
                return false;
        }



    }


}
