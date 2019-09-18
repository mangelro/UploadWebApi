/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 18/09/2019 8:32:23
 *
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using UploadWebApi.Infraestructura.Proceso;

namespace UploadWebApi.Infraestructura.netCDF
{
    /// <summary>
    /// 
    /// </summary>
    public class NetCDFParser
    {


        readonly Dictionary<string, string> _atributos = new Dictionary<string, string>();


        readonly INetCDFConfig _config;



        public NetCDFParser(INetCDFConfig config)
        {
            _config = config;
        }


        public void Procesar(string cdfPath)
        {
            ProcessRunner runner = new ProcessRunner(_config.RutaNcdump, false);

            var exitCode = runner.Run("-x " + cdfPath);


            if(exitCode==0)
                XMLParser(runner.Respuesta);
            else
                throw new NetCDFException($"Error fichero CDF {System.IO.Path.GetFileName(cdfPath)}. {ProcesarError(runner.Error)}");

        }

        void XMLParser(string inputXML)
        {

            using (StringReader stringReader = new StringReader(inputXML))
            using (XmlTextReader reader = new XmlTextReader(stringReader))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "attribute" && reader.Depth==1)
                        {
                            if (!_atributos.ContainsKey(reader.GetAttribute("name")))
                            {
                                _atributos.Add(reader.GetAttribute("name"), reader.GetAttribute("value"));
                            }
                        }
                    }
                }
            }

        }


        public string this[string name] => _atributos[name];




        string ProcesarError(string error)
        {

            int pos = error.IndexOf("NetCDF:");

            if (pos == -1)
                return error;
            else
                return error.Substring(pos);
        }


        //20190416050602+0200
        //2019 04 16 05 06 02 + 02 00
     public static  DateTime GetDateTime(string datetime)
        {
            if (DateTime.TryParseExact(datetime,"yyyyMMddHHmmsszzzz", 
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime ret))
            {
                return ret;
            }

            

            throw new FormatException();
        }



    }
}