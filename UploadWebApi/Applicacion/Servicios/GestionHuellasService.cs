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
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UploadWebApi.Applicacion.Stores;
using UploadWebApi.Models;

namespace UploadWebApi.Applicacion.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public class GestionHuellasService
    {

        readonly IConfiguracionRegistros _conf;
        readonly IHashService _hashService;
        readonly IHuellasStore _store;


        public GestionHuellasService(IConfiguracionRegistros config, IHuellasStore store, IHashService hashService)
        {
            _conf = config ?? throw new ArgumentNullException(nameof(config));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService));
        }


        public async Task<GetHuellaDto> ConsultarHuellaAsync(string idMuestra, Guid idAplicacion)
        {
            var huella = await _store.ReadAsync(idMuestra, idAplicacion);

            ThrowIfNull(idMuestra, huella);

            return new GetHuellaDto { };
        }

        public async Task<GetHuellaDto> CrearRegistroHuellaAsync(InsertHuellaDto dto, Guid idUsuario, Guid idAplicacion)
        {
            var md5 = CalcularMD5(dto.Stream);

            try
            {

                using (TransactionScope tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (md5 == dto.Hash)
                    {

                        HuellaDto insert = new HuellaDto
                        {
                            IdMuestra = dto.IdMuestra,
                            NombreFichero = dto.NombreFichero,
                            FechaHuella = dto.FechaAnalisis,
                            AppCliente = idAplicacion,
                            Propietario = idUsuario,
                            Hash = dto.Hash,
                        };

                        await _store.CreateAsync(insert, dto.Stream);

                        CrearFichero(dto.Stream, dto.NombreFichero);

                    }
                    else
                        throw new ServiceException("La verificación de firmas MD5 no es correcta.");



                    tran.Complete();

                    return new GetHuellaDto
                    {
                        IdMuestra = dto.IdMuestra,
                        FechaAnalisis = dto.FechaAnalisis,
                        NombreFichero = dto.NombreFichero,
                    };

                }//using trans
            }
            catch (IOException)
            {
                throw new ServiceException($"El archivo {dto.NombreFichero} ya existe en el sistema.");
            }
            catch (TransactionAbortedException ex)
            {
                throw new ServiceException($"El archivo {dto.NombreFichero} ya existe en el sistema.");
            }


        }

        public async Task BorrarRegistroHuellaAsync(string idMuestra, Guid idAplicacion)
        {

            var huella = await _store.ReadAsync(idMuestra, idAplicacion);

            ThrowIfNull(idMuestra, huella);

            _store.Delete(huella.IdHuella);

            BorrarFichero(huella.NombreFichero);
        }

        public IEnumerable<GetHuellaDto> ListarHuellas(Guid idUsuario, Guid idAplicacion)
        {
            return new List<GetHuellaDto>();
        }

        public async Task<BlobDto> DownloadHuellaAsync(string idMuestra, Guid idApplicacion)
        {
            var huella = await _store.ReadAsync(idMuestra, idApplicacion);

            ThrowIfNull(idMuestra, huella);
            
            byte[] huellaRaw = await _store.ReadHuellaRawAsync(huella.IdHuella);

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
                throw new NotFoundException($"La huella de la muetra {idMuestra} no existe en el sistema.");
        }



        string CalcularMD5(byte[] buffer)
        {
            return Convert.ToBase64String(_hashService.CalcularHash(buffer));
        }


        /// <summary>
        /// Borra un fichero del sistema de archivos en la ruta establecida
        /// </summary>
        /// <param name="nombreFichero"></param>
        /// <returns></returns>
        bool BorrarFichero(string nombreFichero)
        {
            var fileuploadPath = _conf.RutaFicheros;

            FileInfo file = new FileInfo(Path.Combine(fileuploadPath, nombreFichero));

            if (file.Exists)
            {
                file.Delete();
                return true;
            }
            else
                return false;
        }
        

        /// <summary>
        /// Crea un fichero en el sistema de archivos en la ruta establecida
        /// </summary>
        /// <param name="rawHuella"></param>
        /// <param name="nombreFichero"></param>
        void CrearFichero(byte[] rawHuella,string nombreFichero)
        {
            var fileuploadPath = _conf.RutaFicheros;
            byte[] buffer = new byte[255];

            int leidos = 0;
            using (FileStream file = new FileStream(Path.Combine(fileuploadPath, nombreFichero), FileMode.CreateNew, FileAccess.Write))
            {
                using (var stream = new MemoryStream(rawHuella,false))
                {
                    do
                    {
                        leidos = stream.Read(buffer, 0, buffer.Length);
                        file.Write(buffer, 0, leidos);
                    } while (leidos == buffer.Length);
                }
            }




        }
    }
}