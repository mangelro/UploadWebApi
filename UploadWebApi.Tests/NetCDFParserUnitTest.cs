using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UploadWebApi.Infraestructura.netCDF;

namespace UploadWebApi.Tests
{
    [TestClass]
    public class NetCDFParserUnitTest
    {


        INetCDFConfig _conf;
        public NetCDFParserUnitTest()
        {
            var confMoq = new Mock<INetCDFConfig>();
            confMoq.Setup(c => c.RutaNcdump)
                .Returns(@"C:\Users\miguel\Documents\Visual Studio 2017\Proyectos\UploadWebApi\UploadWebApi\App_Data\netCDF\ncdump.exe");

            _conf = confMoq.Object;
        }



        [TestMethod]
        [ExpectedException(typeof(NetCDFException))]
        public void Parse_FileNotFound()
        {
            var parser = new NetCDFParser(_conf);
            parser.Procesar(@"C:\inetpub\wwwroot\FileUpload\NoExiste.CDF");
        }


        [TestMethod]
        [ExpectedException(typeof(NetCDFException))]
        public void Parse_FormatError()
        {
            var parser = new NetCDFParser(_conf);
            parser.Procesar(@"C:\inetpub\wwwroot\FileUpload\web.config");
        }


        [TestMethod]
        public void Parse_Ok()
        {

            var parser = new NetCDFParser(_conf);

            parser.Procesar(@"C:\inetpub\wwwroot\FileUpload\CIO_2_036.CDF");


            var lan = parser["languages"];


            Assert.AreEqual("English", lan);
        }


        [TestMethod]
        public void Parse_Date()
        {

            var parser = new NetCDFParser(_conf);

            parser.Procesar(@"C:\inetpub\wwwroot\FileUpload\OHO_3_029.CDF");


            var date = parser["netcdf_file_date_time_stamp"];

            var ret = NetCDFParser.GetDateTime(date);

            Console.WriteLine(ret.ToString());

        }


        
    }
}
