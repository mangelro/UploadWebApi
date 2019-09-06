using System.Web.Http;
using System.Web.Http.Cors;

namespace UploadWebApi.Controllers.V1
{

    [RoutePrefix("v1/contrastes")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContrastesController : ApiController
    {
    }
}
