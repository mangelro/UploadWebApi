using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
