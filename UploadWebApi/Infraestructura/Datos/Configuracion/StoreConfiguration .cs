using System.Configuration;
using UploadWebApi.Aplicacion.Stores;

namespace UploadWebApi.Infraestructura.Datos.Configuracion
{
    public class StoreConfiguration : IStoreConfiguration
    {
        public string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }
}