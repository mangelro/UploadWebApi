using System;

namespace UploadWebApi.Aplicacion.Servicios
{
    public interface IConfiguracionRegistros
    {

        string RutaFicheros { get; }

        string RutaTemporal { get; }

        string RutaExeContraste { get; }

        string NombreVector { get; }
    }
}
