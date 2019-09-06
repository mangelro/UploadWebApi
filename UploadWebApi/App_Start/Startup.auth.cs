using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using Owin;


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

                    OnRequestToken = (OAuthRequestTokenContext context) =>
                   {

                       if (context.Request.QueryString.HasValue)
                       {
                           if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"].ToString()))
                           {
                               var queryString = context.Request.QueryString.Value.ParseQuery();

                               var token = queryString["access_token"].ToString();

                               if (!string.IsNullOrWhiteSpace(token))
                               {
                                   context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                               }
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


    static class QueryStringHelper
    {
        public static Dictionary<string, string> ParseQuery(string queryString)
        {
            var result = ParseNullableQuery(queryString);

            if (result == null)
            {
                return new Dictionary<string, string>();
            }

            return result;
        }



        public static Dictionary<string, string> ParseNullableQuery(string queryString)
        {






            if (string.IsNullOrEmpty(queryString) || queryString == "?")
            {
                return null;
            }


        




            int scanIndex = 0;
            if (queryString[0] == '?')
            {
                scanIndex = 1;
            }

            int textLength = queryString.Length;
            int equalIndex = queryString.IndexOf('=');
            if (equalIndex == -1)
            {
                equalIndex = textLength;
            }
            while (scanIndex < textLength)
            {
                int delimiterIndex = queryString.IndexOf('&', scanIndex);
                if (delimiterIndex == -1)
                {
                    delimiterIndex = textLength;
                }
                if (equalIndex < delimiterIndex)
                {
                    while (scanIndex != equalIndex && char.IsWhiteSpace(queryString[scanIndex]))
                    {
                        ++scanIndex;
                    }
                    string name = queryString.Substring(scanIndex, equalIndex - scanIndex);
                    string value = queryString.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);

                    accumulator.Append(
                        Uri.UnescapeDataString(name.Replace('+', ' ')),
                        Uri.UnescapeDataString(value.Replace('+', ' ')));

                    equalIndex = queryString.IndexOf('=', delimiterIndex);

                    if (equalIndex == -1)
                    {
                        equalIndex = textLength;
                    }
                }
                else
                {
                    if (delimiterIndex > scanIndex)
                    {
                        accumulator.Append(queryString.Substring(scanIndex, delimiterIndex - scanIndex), string.Empty);
                    }
                }
                scanIndex = delimiterIndex + 1;
            }

            if (!accumulator.HasValues)
            {
                return null;
            }

            return accumulator.GetResults();

        }
    }



}
