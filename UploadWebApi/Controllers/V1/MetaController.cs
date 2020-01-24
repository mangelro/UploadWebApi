using System;
using System.Diagnostics;
using System.Web.Http;


namespace UploadWebApi.Controllers.V1
{

    [RoutePrefix("v1/meta")]
    public class MetaController : BaseApiController
    {


        [Route("")]
        public IHttpActionResult Get()
        {
            var assembly = typeof(Startup).Assembly;

            var creationDate = System.IO.File.GetCreationTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Ok(new { Version = version, LastUpdated = creationDate });
        }


    


    }
}
