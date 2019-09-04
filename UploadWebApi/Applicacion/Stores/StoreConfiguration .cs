using System.Configuration;

namespace UploadWebApi.Applicacion.Stores
{
    public class StoreConfiguration : IStoreConfiguration
    {


        private readonly string _strConn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;



        public string ConnectionString => _strConn;
    }
}