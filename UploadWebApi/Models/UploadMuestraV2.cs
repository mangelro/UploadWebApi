/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 23/01/2020 16:38:59
 *
 */

using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

using Newtonsoft.Json;

using UploadWebApi.Infraestructura.Serializacion;

namespace UploadWebApi.Models
{

    /*
     * A la hora de subir un fichero junto con datos JSON a un servicio RESTful, 
     * se puede optar por varias alternativa:
     * 
     * - Subirlas de forma independiente. Subir primero de la informaión JSON (application/json) y 
     *   retornar, por ejemplo, junto al mensaje de confimación, la URL para realizar la carga
     *   del fichero: POST:http://api.servcio.es/imagenes/<id_info>/upload.
     *   
     * - Mezclar en un mismo envío la info JSON y el fichero (multipart/form-data). En este caso
     *   la información json no será un objeto serializado sino pares clave/valor. En navegadores 
     *   modernos se puede utilizar el objeto 'FormData'
     *   
     * - Por último se puede subir todo en contenido en formato JSON (application/json) utilizando
     *   la codificación Base64 para el contenido del fichero (la metainformación del fichero como: 
     *   nombre del fichero, tipo, etc. habrá que incluirla explicitamente).
     *   El mayor inconveniente de esta práctica es que el tamaño en bytes necesario para contener
     *   el fichero aumenta un 33%.
     *   
     *      https://dotnetcoretutorials.com/2018/07/21/uploading-images-in-a-pure-json-api/
     */

    public class UploadMuestraV2
    {
        [Required]
        public string IdMuestra { get; set; }

        [Required]
        public DateTime FechaAnalisis { get; set; }

        [Required]
        public string Hash { get; set; }

        [Required]
        [JsonConverter(typeof(Base64FileConverter))]
        public HttpPostedFileBase VectorDatos { get; set; }

        public string Observaciones { get; set; }
    }

}
