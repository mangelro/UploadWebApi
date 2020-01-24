/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 26/06/2019 17:58:28
 *
 */

using System;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UploadWebApi.Infraestructura.Web;

namespace UploadWebApi.Infraestructura.Serializacion
{
    /// <summary>
    /// Convierte una cadenta JSON en un objeto fichero
    /// </summary>
    public class Base64FileConverter : JsonConverter<HttpPostedFileBase>
    {
        public override HttpPostedFileBase ReadJson(JsonReader reader, Type objectType, HttpPostedFileBase existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return new HttpPostedFileB64(jObject.GetValue("FileName").Value<string>(), jObject.GetValue("ContentType").Value<string>(), jObject.GetValue("InputStream").Value<string>());

        }

        public override void WriteJson(JsonWriter writer, HttpPostedFileBase value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}