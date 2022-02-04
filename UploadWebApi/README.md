# Introduccion
/*
     * A la hora de subir un fichero junto con datos JSON a un servicio RESTful (.Net framework 4.x), 
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

# Puntos interesantes de proyecto


    ## Upload ficheros como cadenas de texto B64
    
    /// <summary>
    /// Representa un fichero subido por un cliente en como una cadena de texto formato b64
    /// </summary>
    public class HttpPostedFileB64 : HttpPostedFileBase
    {
        ...
    }


    # Upload DTO con un campo cadena B64 que representa un fichero
    
    /// <summary>
    /// Petición del cliente incluir metainformación del fichero como el nombre y el tipo además puede incluir algunos campos adicionales 
    /// relativos a la lógica de negocio: Id, Observaciones, CRC.
    /// B64 del fichero.
    /// </summary>
    public class DtoRequest
    {
        ...

        [Required]
        [JsonConverter(typeof(Base64FileConverter))]
        public HttpPostedFileBase VectorDatos { get; set; }
    }

    ///
    /// Deserializa la petición del cliente .Incluye un texto con la cadena B64 del fichero se deserializa a un HttpPostedFileB64.
    /// Debe incluir metainformación del fichero como el nombre y el tipo
    ///
    public class Base64FileConverter : JsonConverter<HttpPostedFileBase>
    {

    }

    ## Upload ficheros como multipart/form-data

    Se utiliza HttpPostedFileBase como fichero subido

    Se requier hacer un Binder (IModelBinder)

    ///
    ///
    ///
    public class UploadedFilesModelBinder : AsyncModelBinder {
        ...
    }

    Este binder presenta un problema con los métodos asíncronos para ello implementamos una clase base:

    ///
    ///
    ///
    public abstract class AsyncModelBinder : IModelBinder {
        ...
    }

    Y utlizamos una clase AsyncUtil para ejecutar de manera síncrona metodos asíncronos.


    De manera global podemos registar el Binding:
    
    ///
    /// Registro global en WebApiConfig. Se ha de crear un ModelBinderProvider personalizado
    /// ya que el SimpleModelBinderProvider no me funcionaba
    ///
    config.Services.Insert(typeof(ModelBinderProvider), 0, new UploadedFilesModelBinderProvider());

    Se necesita agregar el atributo [ModelBinder] al parámetro para indicar a la API Web que debe usar un enlazador de modelos y no un formateador de tipo de medio
    ///
    ///
    ///
    public IHttpActionResult Post([ModelBinder] HttpPostedFileBase files){
        ...
    }

    O directamente en la própia declaracion del parámetro:

    ///
    ///
    ///
    public IHttpActionResult Post([ModelBinder(typeof(UploadedFilesModelBinder))] HttpPostedFileBase files)
    {
        ...
    }

    O en la definicion del Tipo IHttpPostedFile:

    ///
    ///
    ///
    [ModelBinder(typeof(UploadedFilesModelBinder))]
    public interface IHttpPostedFile
    {
        ...
    }

