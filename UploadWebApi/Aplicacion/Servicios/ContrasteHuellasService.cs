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

using FundacionOlivar.IO;
using FundacionOlivar.Procesos;
using FundacionOlivar.Validacion;

using UploadWebApi.Aplicacion.Excepciones;
using UploadWebApi.Aplicacion.Modelo;
using UploadWebApi.Aplicacion.Stores;
using UploadWebApi.Infraestructura.Web;
using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public class ContrasteHuellasService
    {

        readonly IConfiguracionRegistros _conf;
        readonly IHuellaAceiteStore _store;
        readonly IHashService _hashService;
        readonly IIdentityService _identityService;


        public ContrasteHuellasService(IConfiguracionRegistros config,IHuellaAceiteStore store, IHashService hashService, IIdentityService identityService)
        {
            _conf = config.ThrowIfNull(nameof(config));
            _store = store.ThrowIfNull(nameof(store));
            _hashService = hashService.ThrowIfNull(nameof(hashService));
            _identityService = identityService.ThrowIfNull(nameof(identityService));
        }

        public async Task<ContrasteDto> ConstrastarHuellas(string idMuestra1, string idMuestra2)
        {

            var tConsulta1 = ConsultarHuella (idMuestra1);

            var tConsulta2 = ConsultarHuella(idMuestra2);

            await Task.WhenAll(tConsulta1, tConsulta2);

            tConsulta1.Result.ThrowIfNull($"La Muestra {idMuestra1} no existe en el sistema.");

            tConsulta2.Result.ThrowIfNull($"La Muestra {idMuestra2} no existe en el sistema.");

            double indice = -1;
            using (var tempFile1 = await CrearFicheroTemporaAsync(tConsulta1.Result.IdMuestra, tConsulta1.Result.IdHuella, tConsulta1.Result.Hash))
            {
                using (var tempFile2 = await CrearFicheroTemporaAsync(tConsulta2.Result.IdMuestra, tConsulta2.Result.IdHuella, tConsulta2.Result.Hash))
                {
                    ProcessRunner runner = new ProcessRunner(_conf.RutaExeContraste,false);

                    var exitCode = runner.Run($"--id1={tempFile1.TempFileName} --id2={tempFile2.TempFileName}");

                    if (exitCode == 0)
                        indice = Double.Parse(runner.Respuesta, System.Globalization.CultureInfo.InvariantCulture);

                }
            }

            await BloquearHuellasAsync(tConsulta1.Result.IdHuella, tConsulta2.Result.IdHuella);

            return new ContrasteDto {
                IdMuestra1= idMuestra1,
                IdMuestra2= idMuestra2,
                FechaContraste = DateTime.UtcNow,
                UmbralAceptacion = _conf.UmbralContraste,
                IndiceSimilitud = indice,
                Estado = (indice >= _conf.UmbralContraste ? EstadoContrasteType.VALIDO : EstadoContrasteType.INVALIDO),
            };
        }

        public async Task BloquearHuellasAsync(int idHuella1, int idHuella2)
        {
            DateTime ahora = DateTime.UtcNow;

            await _store.BloquearAsync(idHuella1, ahora);
            await _store.BloquearAsync(idHuella2, ahora);
        }

        async Task<HuellaAceite> ConsultarHuella(string idMuestra)
        {
            return await _store.ReadAsync(idMuestra, _identityService.UserIdentity,_identityService.AppIdentity);
        }

        async Task<TemporalFile> CrearFicheroTemporaAsync(string idMuestra, int idHuella, string hash)
        {

            var tempFile = new CdfTempFile(_conf.RutaTemporal);

            Stream rawFile = await _store.ReadHuellaRawAsync(idHuella);

            if (!_hashService.VerifyHash(rawFile, hash))
                throw new ServiceException($"La verificación de firmas de la muestra {idMuestra} no es correcta.");

            tempFile.Create(rawFile);

            return tempFile;
        }

    }
}