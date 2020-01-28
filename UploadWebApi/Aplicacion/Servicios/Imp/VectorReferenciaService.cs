/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 27/01/2020 10:10:20
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FundacionOlivar.IO;
using FundacionOlivar.Validacion;
using HuellaDactilarAceite.Escritores;
using HuellaDactilarAceite.Lectores;
using HuellaDactilarAceite.Modelo;
using HuellaDactilarAceite.VectoresReferencia;
using UploadWebApi.Aplicacion.Excepciones;
using UploadWebApi.Aplicacion.Mapeado;
using UploadWebApi.Aplicacion.Modelo;
using UploadWebApi.Aplicacion.Stores;
using UploadWebApi.Infraestructura.Extensiones;
using UploadWebApi.Infraestructura.Ficheros;
using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Servicios.Imp
{
    /// <summary>
    /// 
    /// </summary>
    public class VectorReferenciaService : IVectorReferenciaService
    {

        readonly IConfiguracionRegistros _conf;
        readonly IGestionHuellasService _huellasService;
        readonly IVectorReaderFactory _vectorReaderFactory;
        readonly IVectorWriterFactory _vectorWriterFactory;
        readonly IVectorReferenciaCreatorService _creatorService;

        public VectorReferenciaService(
            IConfiguracionRegistros config,
            IGestionHuellasService huellasService,
            IVectorReaderFactory readerVectorFactory,
            IVectorWriterFactory writerVectorFactory,
            IVectorReferenciaCreatorService creatorService)
        {
            _conf = config.ThrowIfNull(nameof(config));
            _huellasService = huellasService.ThrowIfNull(nameof(huellasService));
            _vectorReaderFactory = readerVectorFactory.ThrowIfNull(nameof(readerVectorFactory));
            _vectorWriterFactory = writerVectorFactory.ThrowIfNull(nameof(writerVectorFactory));
            _creatorService = creatorService.ThrowIfNull(nameof(creatorService));
        }


        public async Task<GetHuellaDto> CrearRegistroHuellaRefenciaAsync(InsertVectorReferencia dto)
        {
            try
            {
                dto.IdMuestras.VerificarDuplicidadId("El identificador de la Muestra '{0}' se encuentra duplicado.");

                var huellas = await ConsultarHuellas(dto.IdMuestras);

                List<VectorHuellaAceite> vectores = new List<VectorHuellaAceite>();
                foreach (var h in huellas)
                {
                    vectores.Add(await GetVectorHuellaAsync(h));
                }

                var referenciaNameFile = dto.IdMuestraReferencia + "_ref.mat";
                var vectorReferencia = _creatorService.CalculoVectorReferecia(vectores);


                IVectorWriter writer = _vectorWriterFactory.Create();


                using (var temp = new TemporalFile())
                {
                    writer.WriteVector(temp.CurrentStream, _conf.NombreVector, vectorReferencia);

                    using (FileStream file = new FileStream(temp.TempFileName, FileMode.Open, FileAccess.Read))
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            stream.Seek(0, SeekOrigin.Begin);

                            var hash = await _huellasService.CalcularHash(stream);
                            System.Diagnostics.Debug.WriteLine(hash);

                            InsertHuellaDto inserted = new InsertHuellaDto()
                            {
                                IdMuestra = dto.IdMuestraReferencia,
                                FechaAnalisis = DateTime.Now,
                                NombreFichero = referenciaNameFile,
                                Hash = hash,
                                FileStream = stream,
                                VectorReferencia = true,
                                Observaciones = "Vector referencia de las muestras " + String.Join(", ", dto.IdMuestras.ToArray()),
                            };

                            return await _huellasService.CrearRegistroHuellaAsync(inserted);
                        }
                    }
                }


            }
            catch (ArgumentException agEx)
            {
                throw new ServiceException(agEx.Message);
            }
        }

        async Task<VectorHuellaAceite> GetVectorHuellaAsync(GetHuellaDto huella)
        {
            IVectorReader vectorReader1 = _vectorReaderFactory.Create(huella.NombreFichero);
            BlobDto fichero = await ConsultarHuellaRaw(huella.IdHuella);

            var vector1 = vectorReader1.ReadVector(fichero.FileStream, _conf.NombreVector);

            return vector1;
        }

        async Task<List<GetHuellaDto>> ConsultarHuellas(IEnumerable<string> idMuestras)
        {
            List<GetHuellaDto> huellas = new List<GetHuellaDto>();

            foreach (var id in idMuestras)
                huellas.Add(await _huellasService.ConsultarHuellaAsync(id));

            return huellas;
        }

        async Task<BlobDto> ConsultarHuellaRaw(int idHuella)
        {
            return await _huellasService.DownloadHuellaAsync(idHuella);
        }



    }
}