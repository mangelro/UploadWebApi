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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UploadWebApi.Infraestructura.Serializacion
{
    /// <summary>
    /// Convierte una cadena dte texto en Base60 a un array de bytes
    /// </summary>
    public class ByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] ReadJson(JsonReader reader, Type objectType, byte[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            byte[] data;

            if (reader.TokenType == JsonToken.String)
            {
                // current token is already at base64 string
                // unable to call ReadAsBytes so do it the old fashion way
                string encodedData = reader.Value.ToString();
                data = Convert.FromBase64String(encodedData);
                return data;
            }

            throw new ArgumentException();
        }


        public override void WriteJson(JsonWriter writer, byte[] value, JsonSerializer serializer)
        {
            writer.WriteRawValue(Convert.ToBase64String(value));
        }
    }
}