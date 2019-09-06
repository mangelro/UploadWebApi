using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.WebApi;
using Owin;
using UploadWebApi.Infraestructura.Autofac;

namespace UploadWebApi
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <see cref="https://github.com/autofac/Examples/tree/master/src/WebApiExample.OwinSelfHost"/> 

    public class AutofacWebapiConfig {

        private static IContainer Container;

        public static void Initialize(HttpConfiguration configuration, IAppBuilder app)
        {
            Initialize(configuration, RegisterServices(new ContainerBuilder()));


            app.UseAutofacMiddleware(AutofacWebapiConfig.Container);
            app.UseAutofacWebApi(configuration);

        }


        static void Initialize(HttpConfiguration configuration, IContainer container)
        {
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        /// <summary>
        /// <see cref="https://github.com/autofac/Examples/blob/master/src/WebApiExample.OwinSelfHost/Startup.cs"/> 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        static IContainer RegisterServices(ContainerBuilder builder)
        {


        
            //Register your Web API controllers.  
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterModule(new ApplicationModule());

            builder.RegisterModule(new InfraestructuraModule());

            Container = builder.Build();

            return Container;
        }



        public static ILifetimeScope GetLifetimeScope()
        {
            return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
        }
    }
}