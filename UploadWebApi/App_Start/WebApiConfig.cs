using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UploadWebApi.Infraestructura;

namespace UploadWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType)); //El HostAuthenticationFilter clase habilita la autenticación con tokens de portador.

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            //config.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter());


            ////De manera global serializamos los Enum como cadenas
            ////https://exceptionnotfound.net/serializing-enumerations-in-asp-net-web-api/
            ////config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            json.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
            json.SerializerSettings.Converters.Add(new StringEnumConverter());
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();




            // Configuración y servicios de API web
            config.EnableCors();
            //config.EnableCors(new EnableCorsAttribute("*", "*", "*"));



            // Rutas de API web
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
