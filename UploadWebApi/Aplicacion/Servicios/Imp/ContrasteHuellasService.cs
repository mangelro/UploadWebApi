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

using FundacionOlivar.Validacion;

using HuellaDactilarAceite.Lectores;
using HuellaDactilarAceite.IndicesSimilitud;

using UploadWebApi.Aplicacion.Modelo;
using UploadWebApi.Aplicacion.Stores;
using UploadWebApi.Models;
using HuellaDactilarAceite.Modelo;
using UploadWebApi.Aplicacion.Excepciones;
using UploadWebApi.Aplicacion.Servicios.Procesadores;
using UploadWebApi.Aplicacion.Validadores;
using UploadWebApi.Infraestructura.Extensiones;

namespace UploadWebApi.Aplicacion.Servicios.Imp
{
    /// <summary>
    /// 
    /// </summary>
    public class ContrasteHuellasService : IContrasteHuellasService
    {

        readonly IConfiguracionRegistros _conf;
        readonly IHuellaAceiteStore _store;
        readonly IHashService _hashService;
        readonly IIdentityService _identityService;
        readonly IVectorReaderFactory _vectorFactory;
        readonly IProcesadorVectores _procesador;
        readonly IValidadorContraste _validador;


        public ContrasteHuellasService(
            IConfiguracionRegistros config,
            IHuellaAceiteStore store, 
            IHashService hashService, 
            IIdentityService identityService, 
            IVectorReaderFactory vectorFactory,
            IProcesadorVectores procesador,
            IValidadorContraste validador)
        {
            _conf = config.ThrowIfNull(nameof(config));
            _store = store.ThrowIfNull(nameof(store));
            _hashService = hashService.ThrowIfNull(nameof(hashService));
            _identityService = identityService.ThrowIfNull(nameof(identityService));
            _vectorFactory= vectorFactory.ThrowIfNull(nameof(vectorFactory));
            _procesador = procesador.ThrowIfNull(nameof(procesador));
            _validador = validador.ThrowIfNull(nameof(validador));
        }

        public async Task<ContrasteDto> ConstrastarHuellasAsync(string idMuestra1, string idMuestra2)
        {

            var tConsulta1 = ConsultarHuella (idMuestra1);

            var tConsulta2 = ConsultarHuella(idMuestra2);

            await Task.WhenAll(tConsulta1, tConsulta2);

            tConsulta1.Result.ThrowIfNull($"La Muestra {idMuestra1} no existe en el sistema.");

            tConsulta2.Result.ThrowIfNull($"La Muestra {idMuestra2} no existe en el sistema.");
            

            var tFichero1 = ConsultarHuellaRaw(tConsulta1.Result.IdHuella);

            var tFichero2 = ConsultarHuellaRaw(tConsulta2.Result.IdHuella);


            await Task.WhenAll(tFichero1, tFichero2);


            IVectorReader vectorReader1 = _vectorFactory.Create(tConsulta1.Result.NombreFichero);
            var vector1 = vectorReader1.ReadVector(tFichero1.Result, _conf.NombreVector);

            IVectorReader vectorReader2 = _vectorFactory.Create(tConsulta2.Result.NombreFichero);
            var vector2 = vectorReader2.ReadVector(tFichero2.Result, _conf.NombreVector);


            IReadOnlyList<ItemIndice> indices;

            indices = _procesador.ProcesarVectores
                (
                    new ItemVector[]
                    {
                        new ItemVector(tConsulta1.Result.IdMuestra, vector1),
                        new ItemVector(tConsulta2.Result.IdMuestra, vector2),
                    }
                ); 

            await BloquearHuellas(tConsulta1.Result.IdHuella, tConsulta2.Result.IdHuella);

            var dto =new ContrasteDto {
                FechaContraste = DateTime.UtcNow,
                ProtocoloIndice= _procesador.Indice.Protocolo,
                UmbralAceptacion = (double)_procesador.Indice.UmbralAceptacion,
                Estado = (indices[0].Indice >= _procesador.Indice.UmbralAceptacion ? EstadoContrasteType.VALIDO : EstadoContrasteType.INVALIDO),
            };

            dto.Contrastes = indices.Select(i => new ItemContrasteDto
            {
                IdMuestra1 = i.IdMuestra1,
                IdMuestra2 = i.IdMuestra2,
                IndiceSimilitud = (double)i.Indice,
            });


            return dto;

          
        }

        public async Task<ContrasteDto> ConstrastarHuellasAsync( IEnumerable<string> muestras)
        {
            try
            {
                muestras.VerificarDuplicidadId("El identificador de la Muestra '{0}' se encuentra duplicado.");

                var huellas = await ConsultarHuellas(muestras);

                var vectores = await ConsultarVectores(huellas.Select(h => h.IdHuella));

                if (huellas.Count != vectores.Count)
                    throw new ServiceException("No existen el mismo número de muestras que de vectores");


                IReadOnlyList<ItemIndice> indices;

                List<ItemVector> items = new List<ItemVector>();

                for (var i = 0; i < huellas.Count; i++)
                {
                    items.Add(new ItemVector(huellas[i].IdMuestra, vectores[i]));
                }

                indices = _procesador.ProcesarVectores(items);

                return MapDto(indices);
            }
            catch (ArgumentException agEx)
            {
                throw new ServiceException(agEx.Message);
            }
        }


        async Task<IReadOnlyList<HuellaAceite>> ConsultarHuellas(IEnumerable<string> muestras)
        {
            List<Task<HuellaAceite>> consultasHuellas = new List<Task<HuellaAceite>>();

            foreach (var id in muestras)
                consultasHuellas.Add(ConsultarHuella(id));

            await Task.WhenAll(consultasHuellas);

            return consultasHuellas.Select(t => t.Result).ToList().AsReadOnly();
        }

        async Task<IReadOnlyList<Stream>> ConsultarHuellasRaw(IEnumerable<int> huellas)
        {
            List<Task<Stream>> consultasHuellas = new List<Task<Stream>>();

            foreach (var id in huellas)
                consultasHuellas.Add(ConsultarHuellaRaw(id));

            await Task.WhenAll(consultasHuellas);

            return consultasHuellas.Select(t => t.Result).ToList().AsReadOnly();
        }

        async Task<IReadOnlyList<VectorHuellaAceite>> ConsultarVectores(IEnumerable<int> huellas)
        {

            var streams = await ConsultarHuellasRaw(huellas);

            return streams.Select(h => 
            {
                IVectorReader vectorReader = _vectorFactory.Create("truco.mat");
                return vectorReader.ReadVector(h, _conf.NombreVector);
            }).ToList().AsReadOnly();

        }



        ContrasteDto MapDto(IEnumerable<ItemIndice> indices)
        {
            var dto = new ContrasteDto
            {
                FechaContraste = DateTime.UtcNow,
                ProtocoloIndice = _procesador.Indice.Protocolo,
                UmbralAceptacion = (double)_procesador.Indice.UmbralAceptacion,
                Estado = _validador.ValidarConstraste(indices) ? EstadoContrasteType.VALIDO : EstadoContrasteType.INVALIDO,
            };

            dto.Contrastes = indices.Select(i => new ItemContrasteDto
            {
                IdMuestra1 = i.IdMuestra1,
                IdMuestra2 = i.IdMuestra2,
                IndiceSimilitud = (double)i.Indice,
                 Estado= i.Validez ? EstadoContrasteType.VALIDO : EstadoContrasteType.INVALIDO,
            });

            return dto;
        }






        async Task BloquearHuellas(int idHuella1, int idHuella2)
        {
            DateTime ahora = DateTime.UtcNow;

            await _store.BloquearAsync(idHuella1, ahora);
            await _store.BloquearAsync(idHuella2, ahora);
        }

        Task<HuellaAceite> ConsultarHuella(string idMuestra)
        {
            return _store.ReadAsync(idMuestra, _identityService.UserIdentity,_identityService.AppIdentity);
        }

        Task<Stream> ConsultarHuellaRaw(int idHuella)
        {
            return _store.ReadHuellaRawAsync(idHuella);
        }
    }
}