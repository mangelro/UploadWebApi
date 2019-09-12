using System;



namespace UploadWebApi.Applicacion
{
    public interface IHashService
    {

        byte[] CalcularHash(byte[] buffer);


        bool VerifyHash(string hashEsperado, byte[] raw);

    }
}
