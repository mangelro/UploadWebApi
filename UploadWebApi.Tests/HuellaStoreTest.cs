using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UploadWebApi.Applicacion.Stores;
using UploadWebApi.Models;

namespace UploadWebApi.Tests
{
    [TestClass]
    public class HuellaStoreTest
    {
        const string texto = "There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum, you need to be sure there isn't anything embarrassing hidden in the middle of text. All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary, making this the first true generator on the Internet. It uses a dictionary of over 200 Latin words, combined with a handful of model sentence structures, to generate Lorem Ipsum which looks reasonable. The generated Lorem Ipsum is therefore always free from repetition, injected humour, or non-characteristic words etc.";

        readonly IStoreConfiguration _conf;

        public HuellaStoreTest()
        {
            var confMoq = new Mock<IStoreConfiguration>();
            confMoq.Setup(c => c.ConnectionString)
                .Returns("Data Source=.\\SQLEXPRESS;Initial Catalog=INTERPANEL20;Integrated Security=false;User ID=INTERuser;Password=INTERpass!2pz;Application Name=interpanel");

            _conf = confMoq.Object;

        }

        [TestMethod]
        public async Task Crear_Huella_OK()
        {

            var bytes = System.Text.Encoding.UTF8.GetBytes(texto);
            IHuellasStore store = new DapperHuellasStore(_conf);

            HuellaDto dto = new HuellaDto
            {
                FechaHuella = DateTime.Now,
                AppCliente = Guid.Parse("75AB4073-D7F5-4EB7-BBAB-2ABBE59D228F"),
                IdMuestra = "XX-33",
                NombreFichero = "OHO_3_029.CDF",
                Propietario = Guid.Parse("53d10481-74ad-4223-a4e1-75312847a33f"),
                Hash = "53d10481-74ad-4223-a4e1-75312847a33f",
            };


            await store.CreateAsync(dto,bytes);

        }


        [TestMethod]
        public async Task Escribir_Stream_OK()
        {


            IHuellasStore store = new DapperHuellasStore(_conf);

            var bytes = System.Text.Encoding.UTF8.GetBytes(texto);

           await store.WriteHuellaRawAsync(1,bytes);
            

        }




        [TestMethod]
        public async Task Leer_Stream_OK()
        {


            IHuellasStore store = new DapperHuellasStore(_conf);

            byte[] salida= await store.ReadHuellaRawAsync(3);

            var text0 = System.Text.Encoding.UTF8.GetString(salida);

        }



    }
}
