/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 20/01/2020 16:55:06
 *
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Http.Validation;
using System.Web.Http.Validation.Providers;
using System.Web.Http.ValueProviders.Providers;

using UploadWebApi.Infraestructura.Web;

namespace UploadWebApi.Infraestructura.Binding
{
    /// <summary>
    /// Enlaza el modelo de datos a través de un ContentType multipart/form-data donde
    /// se mezclan items clave/valor y un ficheros
    /// 
    /// Este sistema es más eficiente a la hora de subir grandes ficheros.
    /// 
    /// <see cref="https://gist.github.com/Danielku15/bfc568a19b9e58fd9e80#file-formmultipartencodedmediatypeformatter-cs"/>
    /// </summary>
    public class FormMultipartEncodedMediaTypeFormatter: MediaTypeFormatter
    {
        const string SupportedMediaType = "multipart/form-data";


        public FormMultipartEncodedMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(SupportedMediaType));

            // New code:
            SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            SupportedEncodings.Add(Encoding.GetEncoding("iso-8859-1"));
        }

        public override bool CanReadType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }


        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (readStream == null) throw new ArgumentNullException(nameof(readStream));

            try
            {
                // load multipart data into memory 
                var multipartProvider = await content.ReadAsMultipartAsync();
                // fill parts into a ditionary for later binding to model
                var modelDictionary = await ToModelDictionaryAsync(multipartProvider);
                //// bind data to model 
                return BindToModel(modelDictionary, type, formatterLogger);
            }
            catch (Exception e)
            {
                if (formatterLogger == null)
                {
                    throw;
                }
                formatterLogger.LogError(string.Empty, e);
                return GetDefaultValueForType(type);
            }
        }


        private async Task<IDictionary<string, object>> ToModelDictionaryAsync(MultipartMemoryStreamProvider multipartProvider)
        {
            var dictionary = new Dictionary<string, object>();

            // iterate all parts 
            foreach (var part in multipartProvider.Contents)
            {
                // unescape the name 
                var name = part.Headers.ContentDisposition.Name.Trim('"');

                // if we have a filename, we treat the part as file upload,
                // otherwise as simple string, model binder will convert strings to other types. 
                if (!string.IsNullOrEmpty(part.Headers.ContentDisposition.FileName))
                {
                    // set null if no content was submitted to have support for [Required]
                    if (part.Headers.ContentLength.GetValueOrDefault() > 0)
                    {
                        dictionary[name] = new HttpPostedFileMultipart(
                            part.Headers.ContentDisposition.FileName.Trim('"'),
                            part.Headers.ContentType.MediaType,
                            await part.ReadAsByteArrayAsync()
                        );
                    }
                    else
                    {
                        dictionary[name] = null;
                    }
                }
                else
                {
                    dictionary[name] = await part.ReadAsStringAsync();
                }
            }

            return dictionary;
        }

        private object BindToModel(IDictionary<string, object> data, Type type, IFormatterLogger formatterLogger)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (type == null) throw new ArgumentNullException(nameof(type));

            using (var config = new HttpConfiguration())
            {
                // if there is a requiredMemberSelector set, use this one by replacing the validator provider
                var validateRequiredMembers = RequiredMemberSelector != null && formatterLogger != null;
                if (validateRequiredMembers)
                {
                    config.Services.Replace(typeof(ModelValidatorProvider), new RequiredMemberModelValidatorProvider(RequiredMemberSelector));
                }

                // create a action context for model binding
                var actionContext = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Configuration = config,
                        ControllerDescriptor = new HttpControllerDescriptor
                        {
                            Configuration = config
                        }
                    }
                };

                // create model binder context 
                var valueProvider = new NameValuePairsValueProvider(data, CultureInfo.InvariantCulture);
                var metadataProvider = actionContext.ControllerContext.Configuration.Services.GetModelMetadataProvider();
                var metadata = metadataProvider.GetMetadataForType(null, type);
                var modelBindingContext = new ModelBindingContext
                {
                    ModelName = string.Empty,
                    FallbackToEmptyPrefix = false,
                    ModelMetadata = metadata,
                    ModelState = actionContext.ModelState,
                    ValueProvider = valueProvider
                };

                // bind model 
                var modelBinderProvider = new CompositeModelBinderProvider(config.Services.GetModelBinderProviders());
                var binder = modelBinderProvider.GetBinder(config, type);
                var haveResult = binder.BindModel(actionContext, modelBindingContext);

                // log validation errors 
                if (formatterLogger != null)
                {
                    foreach (var modelStatePair in actionContext.ModelState)
                    {
                        foreach (var modelError in modelStatePair.Value.Errors)
                        {
                            if (modelError.Exception != null)
                            {
                                formatterLogger.LogError(modelStatePair.Key, modelError.Exception);
                            }
                            else
                            {
                                formatterLogger.LogError(modelStatePair.Key, modelError.ErrorMessage);
                            }
                        }
                    }
                }

                return haveResult ? modelBindingContext.Model : GetDefaultValueForType(type);
            }
        }
    }

    //private object BindToModel(IDictionary<string, object> data, Type type, IFormatterLogger formatterLogger)
    //{
    //    if (data == null) throw new ArgumentNullException(nameof(data));
    //    if (type == null) throw new ArgumentNullException(nameof(type));

    //    //var valueProvider = new NameValuePairsValueProvider(data, CultureInfo.InvariantCulture);

    //    return (new BuilderDto(type)).Build(data);
    //}
}

