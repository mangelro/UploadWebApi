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
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FundacionOlivar.Validacion;
using FundacionOlivar.Web.Modelos;

using UploadWebApi.Aplicacion.Mapeado;
using UploadWebApi.Aplicacion.Stores;
using UploadWebApi.Aplicacion.Modelo;
using UploadWebApi.Aplicacion.Excepciones;
using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Servicios.Imp
{
    /// <summary>
    /// 
    /// </summary>
    public class GestionHuellasService: IGestionHuellasService
    {

        readonly IConfiguracionRegistros _conf;
        readonly IHuellaAceiteStore _store;
        readonly IHashService _hashService;
        readonly IMapperService _mapperService;
        readonly IIdentityService _identityService;

        public GestionHuellasService(IConfiguracionRegistros config, IHuellaAceiteStore store, IHashService hashService, IMapperService mapperService, IIdentityService identityService)
        {
            _conf = config.ThrowIfNull(nameof(config));
            _store = store.ThrowIfNull(nameof(store));
            _hashService = hashService.ThrowIfNull(nameof(hashService));
            _identityService = identityService.ThrowIfNull(nameof(identityService));
            _mapperService = mapperService.ThrowIfNull(nameof(mapperService));

        }

        /// <summary>
        /// Verifica la validez (firma y formato) del fichero
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Task VerificarFichero(Stream rawFile, string hash)
        {

            if (!_hashService.VerifyHash(rawFile, hash))
                throw new ServiceException($"La verificación de firmas de la huella no es correcta.");


            //using (var tempFile = new CdfTempFile(_conf.RutaTemporal))
            //{

            //    tempFile.Create(rawFile);

            //    var parser = new NetCDFParser(new NetCDFConfig());

            //    try
            //    {
            //        parser.Procesar(tempFile.TempFileName);
            //    }
            //    catch (NetCDFException ex)
            //    {
            //        throw new ServiceException("El fichero CDF no tiene el formato correcto.");
            //    }

            //    return Task.CompletedTask;
            //}

            return Task.CompletedTask;
        }

        public async Task<Tuple<IEnumerable<GetRowHuellaDto>, int>> ConsultarHuellasAsync(RangoPaginacion paginacion, OrdenType orden)
        {
            try
            {
                var huellas = await _store.ReadAllAsync(paginacion, _identityService.UserIdentity, _identityService.AppIdentity, orden);

                var dtos = huellas.Item1.Select(h => _mapperService.Map<HuellaAceite, GetRowHuellaDto>(h));

                return Tuple.Create<IEnumerable<GetRowHuellaDto>, int>(dtos, huellas.Item2);
            }
            catch (ArgumentException ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }

        public async Task<GetHuellaDto> ConsultarHuellaAsync(string idMuestra)
        {


            //Si se trata de un usuario Administrador puede acceder a todas
            //las de la APP
            var huella = await _store.ReadAsync(idMuestra, _identityService.IsSysAdmin? Guid.Empty : _identityService.UserIdentity, _identityService.AppIdentity);

            var dto = _mapperService.Map<HuellaAceite, GetHuellaDto>(huella);

            return dto;
        }

        public async Task<GetHuellaDto> CrearRegistroHuellaAsync(InsertHuellaDto dto)
        {
            try
            {
                HuellaAceite inserted = null;

                using (TransactionScope tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {


                    inserted = new HuellaAceite
                    {
                        IdMuestra = dto.IdMuestra,
                        NombreFichero = dto.NombreFichero,
                        FechaAnalisis = dto.FechaAnalisis,
                        AppCliente = _identityService.AppIdentity,
                        Propietario = _identityService.UserIdentity,
                        Hash = dto.Hash,
                        Observaciones = dto.Observaciones
                    };

                    await _store.CreateAsync(inserted, dto.FileStream);

                    var fileuploadPath = _conf.RutaFicheros;

                    CrearFichero(dto.FileStream, Path.Combine(fileuploadPath, GetFormatoNombre(dto.NombreFichero, inserted.IdHuella)));

                    tran.Complete();
                }//using trans

                return _mapperService.Map<HuellaAceite,GetHuellaDto>(await _store.ReadAsync(inserted.IdHuella));
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

        public async Task BorrarRegistroHuellaAsync(string idMuestra)
        {

            HuellaAceite huella = null;


            bool forzarBorrado = _identityService.IsSysAdmin;

            try
            {


                huella = await _store.ReadAsync(idMuestra, _identityService.UserIdentity, _identityService.AppIdentity);

                if (huella.EstaBloqueada && !forzarBorrado)
                {
                    throw new ServiceException($"La huella {idMuestra} está bloqueada y no puede ser eliminada.");
                }


                using (TransactionScope tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

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

        public async Task<BlobDto> DownloadHuellaAsync(string idMuestra)
        {
            var huella = await _store.ReadAsync(idMuestra, _identityService.UserIdentity, _identityService.AppIdentity);

            return await DownloadHuellaInternalAsync(huella);
        }

        public async Task<BlobDto> DownloadHuellaAsync(int idHuella)
        {

            var huella = await _store.ReadAsync(idHuella);

            return await DownloadHuellaInternalAsync(huella);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="huella"></param>
        /// <returns></returns>
        async Task<BlobDto> DownloadHuellaInternalAsync(HuellaAceite huella)
        {
            Stream huellaRaw = await _store.ReadHuellaRawAsync(huella.IdHuella);

            if (!_hashService.VerifyHash(huellaRaw, huella.Hash))
                throw new ServiceException($"La verificación de firmas de la muestra {huella.IdMuestra} no es correcta.");


            return new BlobDto
            {
                FileStream = huellaRaw,
                NombreFichero = huella.NombreFichero,
                Hash = huella.Hash,
            };
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
        static void CrearFichero(Stream rawHuella, string rutaFichero)
        {
            byte[] buffer = new byte[255];

            int leidos = 0;
            using (FileStream file = new FileStream(rutaFichero, FileMode.CreateNew, FileAccess.Write))
            {

                while ((leidos = rawHuella.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, leidos);
                }

                file.Flush();
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
