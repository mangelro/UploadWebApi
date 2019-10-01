using System;



namespace UploadWebApi.Applicacion
{
    public interface IHashService
    {

        byte[] CalcularHash(byte[] buffer);


        bool VerifyHash(byte[] raw, string hashEsperado);

    }
}
