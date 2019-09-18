using System.Configuration;
using UploadWebApi.Applicacion.Stores;

namespace UploadWebApi.Infraestructura.Configuracion
{
    public class StoreConfiguration : IStoreConfiguration
    {


        private readonly string _strConn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;



        public string ConnectionString => _strConn;
    }
}