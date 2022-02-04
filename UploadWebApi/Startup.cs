using System.Web.Http;
using Microsoft.Owin;
using Owin;
using System.Web.Hosting;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Diagnostics;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using System.Web;
using UploadWebApi.Infraestructura.Binding;

[assembly: OwinStartup(typeof(UploadWebApi.Startup))]
namespace UploadWebApi
{


    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {


            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var configuration = new HttpConfiguration();


            //Configurar AutoFac  
            AutofacWebapiConfig.Initialize(configuration, app);



            WebApiConfig.Register(configuration);


            ConfigureOAuth(app);

            //Ha de ser la última llamada
            app.UseWebApi(configuration);




        }


    }

}
