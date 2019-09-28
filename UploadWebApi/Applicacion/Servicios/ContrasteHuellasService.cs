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
using UploadWebApi.Infraestructura.Proceso;
using UploadWebApi.Infraestructura.Web;
using UploadWebApi.Models;

namespace UploadWebApi.Applicacion.Servicios
{
    /// <summary>
    /// 
    /// </summary>
    public class ContrasteHuellasService
    {

        readonly IConfiguracionRegistros _conf;
        readonly IHuellasStore _store;
        readonly IHashService _hashService;


        public ContrasteHuellasService(IConfiguracionRegistros config,IHuellasStore store, IHashService hashService)
        {
            _conf = config ?? throw new ArgumentNullException(nameof(config));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService));
        }


        public async Task<ContrasteDto> ConstrastarHuellas(string idMuestra1, string idMuestra2, Guid idAplicacion)
        {

            var tConsulta1 = ConsultarHuella (idMuestra1, idAplicacion);


            var tConsulta2 = ConsultarHuella(idMuestra2, idAplicacion);


            await Task.WhenAll(tConsulta1, tConsulta2);


            ThrowIfNull(idMuestra1, tConsulta1.Result);

            ThrowIfNull(idMuestra2, tConsulta2.Result);



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

               
        void ThrowIfNull(string idMuestra, HuellaDto huella)
        {
            if (huella == null)
                throw new NotFoundException($"La huella de la muestra {idMuestra} no existe en el sistema.");
        }

        Task<HuellaDto> ConsultarHuella(string idMuestra,Guid idAplicacion)
        {
            return _store.ReadAsync(idMuestra, Guid.Empty, idAplicacion);
           
        }

        async Task<CdfTempFile> CrearFicheroTemporaAsync(string idMuestra, int idHuella,string hash)
        {

            var tempFile = new CdfTempFile(_conf.RutaTemporal);

            byte[] raw = await _store.ReadHuellaRawAsync(idHuella);
            if (!_hashService.VerifyHash(hash, raw))
                throw new ServiceException($"La verificación de firmas de la muestra {idMuestra} no es correcta.");

            tempFile.Create(raw);

            return tempFile;
        }

        public async Task BloquearHuellasAsync(int idHuella1, int idHuella2)
        {
            DateTime ahora = DateTime.UtcNow;

            await _store.BloquearAsync(idHuella1, ahora);
            await _store.BloquearAsync(idHuella2, ahora);
        }

    }
}