using Microsoft.Owin.Security.OAuth;
using Owin;


namespace UploadWebApi
{


    public partial class Startup {



        public void ConfigureOAuth(IAppBuilder app)
        {

            //https://github.com/Legends/BearerTokenDeserializer/tree/Initial/BearerOAuthTokenDeserializer/API
            var oAuthBearerOptions = new OAuthBearerAuthenticationOptions
            {

                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
            };

            //Token Consumption
            app.UseOAuthBearerAuthentication(oAuthBearerOptions);

        }

    }
}
