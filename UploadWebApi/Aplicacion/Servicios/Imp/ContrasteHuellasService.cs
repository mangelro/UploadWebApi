/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:12:53
 *
 */

using System;
using System.IO;
using System.Threading.Tasks;

using FundacionOlivar.Validacion;

using HuellaDactilarAceite.Lectores;
using HuellaDactilarAceite.Similitud;

using UploadWebApi.Aplicacion.Modelo;
using UploadWebApi.Aplicacion.Stores;
using UploadWebApi.Models;

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
        readonly IIndiceSimilitud _indice;

        public ContrasteHuellasService(
            IConfiguracionRegistros config,
            IHuellaAceiteStore store, 
            IHashService hashService, 
            IIdentityService identityService, 
            IVectorReaderFactory vectorFactory,
            IIndiceSimilitud indice)
        {
            _conf = config.ThrowIfNull(nameof(config));
            _store = store.ThrowIfNull(nameof(store));
            _hashService = hashService.ThrowIfNull(nameof(hashService));
            _identityService = identityService.ThrowIfNull(nameof(identityService));
            _vectorFactory= vectorFactory.ThrowIfNull(nameof(vectorFactory));
            _indice = indice.ThrowIfNull(nameof(indice));
        }

        public async Task<ContrasteDto> ConstrastarHuellas(string idMuestra1, string idMuestra2)
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
            var vector1 = vectorReader1.ReadVector<double>(tFichero1.Result, _conf.NombreVector);

            IVectorReader vectorReader2 = _vectorFactory.Create(tConsulta2.Result.NombreFichero);
            var vector2 = vectorReader2.ReadVector<double>(tFichero2.Result, _conf.NombreVector);


            double indice = -1;

            indice=_indice.Similitud(vector1, vector2);

            await BloquearHuellasAsync(tConsulta1.Result.IdHuella, tConsulta2.Result.IdHuella);

            return new ContrasteDto {
                IdMuestra1= idMuestra1,
                IdMuestra2= idMuestra2,
                FechaContraste = DateTime.UtcNow,
                ProtocoloIndice= _indice.Protocolo,
                UmbralAceptacion = _indice.IndiceAceptacion,
                IndiceSimilitud = indice,
                Estado = (indice >= _indice.IndiceAceptacion ? EstadoContrasteType.VALIDO : EstadoContrasteType.INVALIDO),
            };
        }

        async Task BloquearHuellasAsync(int idHuella1, int idHuella2)
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