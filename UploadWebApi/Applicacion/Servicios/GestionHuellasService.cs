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

        public GestionHuellasService(IConfiguracionRegistros config, IHashService hashService)
        {
            _conf = config ?? throw new ArgumentNullException(nameof(config));
            _hashService =hashService ?? throw new ArgumentNullException(nameof(hashService));
        }
        

        public GetHuellaDto ConsultarHuella(string idMuestra)
        {
            return new GetHuellaDto { IdMuestra = idMuestra, FechaAnalisis = DateTime.Now};
        }

        public GetHuellaDto CrearRegistroHuella(InsertHuellaDto dto)
        {

            var fileuploadPath = _conf.RutaFicheros;

            var md5 = CalcularMD5(dto.Stream);

            try
            {
                if (md5 == dto.Hash)
                {
                    using (FileStream file = new FileStream(Path.Combine(fileuploadPath, dto.NombreFichero), FileMode.CreateNew))
                    {
                        file.Write(dto.Stream, 0, dto.Stream.Length);
                    }

                    return new GetHuellaDto { IdMuestra = dto.IdMuestra, FechaAnalisis = dto.FechaAnalisis };
                }
                else
                    throw new ServiceException("La verificación de firmas MD5 no es correcta.");
            }
            catch (IOException)
            {
                throw new ServiceException($"El archivo {dto.NombreFichero} ya existe en el sistema.");
            }
        }

        public void BorrarRegistroHuella(string idMuestra)
        {
            var fileuploadPath = _conf.RutaFicheros;
            //Consulta BD

            var huella = ConsultarHuella(idMuestra);

            string nombreFichero = huella.NombreFichero;

            FileInfo file = new FileInfo(Path.Combine(fileuploadPath, nombreFichero));

            if (file.Exists)
                file.Delete();
            else
                throw new ServiceException($"El archivo {nombreFichero} no existe en el fichero.");
        }

        public IEnumerable<GetHuellaDto> ListarHuellas(Guid idUsuario,Guid idAplicacion)
        {
            return new List<GetHuellaDto>();
        }

        string CalcularMD5(byte[] buffer)
        {
            return Convert.ToBase64String(_hashService.CalcularHash(buffer));
        }
    }
}