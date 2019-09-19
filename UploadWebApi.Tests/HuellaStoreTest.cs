using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UploadWebApi.Applicacion.Stores;
using UploadWebApi.Infraestructura.Servicios;
using UploadWebApi.Models;

namespace UploadWebApi.Tests
{
    [TestClass]
    public class HuellaStoreTest
    {

        readonly IStoreConfiguration _conf;

        public HuellaStoreTest()
        {
            var confMoq = new Mock<IStoreConfiguration>();
            confMoq.Setup(c => c.ConnectionString)
                .Returns("Data Source=IBMX3620\\IBMX3620;Initial Catalog=INTERPANEL20BAK;User ID=INTERuser;Password=INTERpass!2pz;Application Name=interpanel");

            _conf = confMoq.Object;

        }

        [TestMethod]
        public async Task Crear_Huella_OK()
        {

            var bytes = LeerFichero(@"C:\Users\miguel\Documents\Visual Studio 2017\Proyectos\UploadWebApi\UploadWebApi.Tests\ficheros\entrada.pdf");
            IHuellasStore store = new DapperHuellasStore(_conf, new FakeCompresionService());

            HuellaDto dto = new HuellaDto
            {
                FechaAnalisis = DateTime.Now,
                AppCliente = Guid.Parse("75AB4073-D7F5-4EB7-BBAB-2ABBE59D228F"),
                IdMuestra = "XX-33",
                NombreFichero = "OHO_3_029.CDF",
                Propietario = Guid.Parse("53d10481-74ad-4223-a4e1-75312847a33f"),
                Hash = "53d10481-74ad-4223-a4e1-75312847a33f",
            };


            await store.CreateAsync(dto, bytes);

        }


        [TestMethod]
        public async Task Escribir_Stream_Huella_OK()
        {


            IHuellasStore store = new DapperHuellasStore(_conf, new FakeCompresionService());

            var bytes = LeerFichero("C:\\entrada.pdf");

            await store.WriteHuellaRawAsync(1, bytes);

        }




        [TestMethod]
        public async Task Leer_Stream_Huella_OK()
        {


            IHuellasStore store = new DapperHuellasStore(_conf, new FakeCompresionService());

            byte[] salida = await store.ReadHuellaRawAsync(10);

            EscribirFichero(salida, @"C:\Users\miguel\Documents\Visual Studio 2017\Proyectos\UploadWebApi\UploadWebApi.Tests\ficheros\salida.pdf");
        }










        void EscribirFichero(byte[] raw, string rutaFichero)
        {
            byte[] buffer = new byte[255];

            int leidos=0;
            using (var f = new FileStream(rutaFichero, FileMode.Create, FileAccess.Write))
            {
                using (var s = new MemoryStream(raw, 0, raw.Length, false))
                {
                    do
                    {
                        leidos = s.Read(buffer, 0, buffer.Length);
                        f.Write(buffer, 0, leidos);
                    } while (leidos == buffer.Length);
                }
            }
        }

        byte[] LeerFichero(string rutaFichero)
        {

            byte[] buffer = new byte[255];

            int leidos = 0;
            using (var s = new MemoryStream())
            {
                using (var f = new FileStream(rutaFichero, FileMode.Open, FileAccess.Read))
                {
                    do
                    {
                        leidos = f.Read(buffer, 0, buffer.Length);
                        s.Write(buffer, 0, leidos);
                    } while (leidos == buffer.Length);
                }
                return s.ToArray();
            }

        }

    }
}
