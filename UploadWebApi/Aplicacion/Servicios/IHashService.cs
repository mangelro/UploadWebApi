using System;
using System.IO;

namespace UploadWebApi.Aplicacion.Servicios
{
    public interface IHashService
    {

        byte[] CalcularHash(Stream file);


        bool VerifyHash(Stream file, string hashEsperado);

    }
}
