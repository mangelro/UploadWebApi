/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 26/06/2019 17:58:28
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UploadWebApi.Infraestructura.Serializacion
{
    /// <summary>
    /// Convierte una cadena de texto en Base64 a un array de bytes
    /// </summary>
    public class Base64StreamConverter : JsonConverter<Stream>
    {
        public override Stream ReadJson(JsonReader reader, Type objectType, Stream existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }


            if (reader.TokenType == JsonToken.String)
            {
                byte[] buffer;

                string encodedData = reader.Value.ToString();
                buffer = Convert.FromBase64String(encodedData);


                MemoryStream stream = new MemoryStream(buffer,false);


                return stream;
            }

            throw new ArgumentException();
        }


        public override void WriteJson(JsonWriter writer, Stream value, JsonSerializer serializer)
        {
            // writer.WriteRawValue(Convert.ToBase64String(value));
            throw new NotImplementedException();
        }
    }
}