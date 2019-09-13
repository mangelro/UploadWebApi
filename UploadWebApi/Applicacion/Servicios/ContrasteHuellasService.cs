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
            using (var tempFile1 = new CdfTempFile(_conf.RutaTemporal))
            {
                byte[] raw1 = await _store.ReadHuellaRawAsync(tConsulta1.Result.IdHuella);
                if (!_hashService.VerifyHash(tConsulta1.Result.Hash, raw1))
                    throw new ServiceException($"La verificación de firmas de la muestra {idMuestra1} no es correcta.");

                tempFile1.Create(raw1);

                using (var tempFile2 = new CdfTempFile(_conf.RutaTemporal))
                {
                    byte[] raw2 = await _store.ReadHuellaRawAsync(tConsulta2.Result.IdHuella);
                    if (!_hashService.VerifyHash(tConsulta2.Result.Hash, raw2))
                        throw new ServiceException($"La verificación de firmas de la muestra {idMuestra2} no es correcta.");

                    tempFile2.Create(raw2);


                    //indice = await CompareFiles(tempFile1.TempFileName, tempFile2.TempFileName);


                    ProcessRunner runner = new ProcessRunner(@"C:\Users\miguel\Documents\Visual Studio 2017\Proyectos\ContrasteStub\ContrasteStub\bin\Debug\ContrasteStub.exe", $"--id1={tempFile1.TempFileName} --id2={tempFile2.TempFileName}", false);
                    var exitCode = runner.Run();

                    if (exitCode == 0)
                        indice = Double.Parse(runner.Respuesta, System.Globalization.CultureInfo.InvariantCulture);

                }
            }
            
            return new ContrasteDto {
                 IndiceSimilitud=indice,
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

        Task<double> CompareFiles(string file1, string file2)
        {
            return Task.FromResult(0.99);
        }

    
    }
}