using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Applicacion.Servicios
{
    public interface IConfiguracionRegistros
    {

        string RutaFicheros { get; }

        string RutaTemporal { get; }
        
    }
}
