using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UploadWebApi.Infraestructura.Proceso;

namespace UploadWebApi.Tests
{
    [TestClass]
    public class ProcessUnitTest
    {

        [TestMethod]
        public void Ejecutar_Cancelar()
        {


            CancellationTokenSource source = new CancellationTokenSource();

            ProcessRunner runner = new ProcessRunner(@"C:\Users\miguel\Documents\Visual Studio 2017\Proyectos\ContrasteStub\ContrasteStub\bin\Debug\ContrasteStub.exe", "--id1=file1.txt --id2=file2.txt", false);

            var r=runner.RunAsync(source.Token)
                .ContinueWith(t=>
                {

                    if (t.IsCanceled)
                        Console.WriteLine("CANCELADA");
                    else
                    {

                        int exitCode = t.Result;
                        if (exitCode == 0)
                            Console.WriteLine("Respuesta: " + runner.Respuesta);
                        else
                            Console.WriteLine("Error: " + runner.Error);
                    }
                });

          //  source.Cancel();
            r.Wait();

            Console.WriteLine("Saliendo...");
        }


        [TestMethod]
        public async Task Ejecutar_OKAsync()
        {

            ProcessRunner runner = new ProcessRunner(@"C:\Users\miguel\Documents\Visual Studio 2017\Proyectos\ContrasteStub\ContrasteStub\bin\Debug\ContrasteStub.exe", "--id1=file1.txt --id2=file2.txt", false);

            var exitCode = await runner.RunAsync();

            Console.WriteLine("Exit Code: " + exitCode.ToString());
            if (exitCode == 0)
                Console.WriteLine("Respuesta: " + runner.Respuesta);
            else
                Console.WriteLine("Error: " + runner.Error);

        }

        [TestMethod]
        public void Ejecutar_OKSync()
        {

            ProcessRunner runner = new ProcessRunner(@"C:\Users\miguel\Documents\Visual Studio 2017\Proyectos\ContrasteStub\ContrasteStub\bin\Debug\ContrasteStub.exe", "--id1=file1.txt --id2=file2.txt", false);

            var exitCode = runner.Run();

            Console.WriteLine("Exit Code: " + exitCode.ToString());
            if (exitCode == 0)
                Console.WriteLine("Respuesta: " + runner.Respuesta);
            else
                Console.WriteLine("Error: " + runner.Error);

        }

    }
}
