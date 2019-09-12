using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using Owin;
using UploadWebApi.Infraestructura.Web;

namespace UploadWebApi
{


    public partial class Startup
    {



        public void ConfigureOAuth(IAppBuilder app)
        {

            //https://github.com/Legends/BearerTokenDeserializer/tree/Initial/BearerOAuthTokenDeserializer/API
            var oAuthBearerOptions = new OAuthBearerAuthenticationOptions
            {

                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                Provider = new OAuthBearerAuthenticationProvider
                {

                    //Permite localizar el token en el QueryString
                    OnRequestToken = (OAuthRequestTokenContext context) =>
                    {

                        if (context.Request.QueryString.HasValue)
                        {
                            if (!context.Request.Headers.Keys.Contains("Authorization"))
                            {
                                var queryString = QueryStringHelper.ParseQuery(context.Request.QueryString.Value);

                                var token = queryString["accesstoken"];

                                context.Token = token;

                            }

                        }

                        return Task.FromResult<object>(null);
                    }

                },


            };

            //Token Consumption
            app.UseOAuthBearerAuthentication(oAuthBearerOptions);

        }

    }




}
