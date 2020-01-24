using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

using UploadWebApi.Applicacion.Servicios;
using UploadWebApi.Infraestructura.Servicios;
using UploadWebApi.Models;

namespace UploadWebApi.Controllers.V1
{
    [Authorize(Roles = "administradores,contrastadores")]
    [RoutePrefix("v1/contrastar")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContrasteController : BaseApiController
    {
        readonly IIdentityService _identity;
        readonly ContrasteHuellasService _service;
        public ContrasteController(ContrasteHuellasService service, IIdentityService identity)
        {
            _service= service ?? throw new ArgumentNullException(nameof(service));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(ContrasteDto))]
        public async Task<IHttpActionResult> Get(string idMuestra1, string idMuestra2)
        {
            try
            {
                var contraste = await _service.ConstrastarHuellas(idMuestra1, idMuestra2, _identity.AppIdentity);
                return Ok(contraste);
            }
            catch (NotFoundException noEx)
            {
                return BadRequest(noEx.Message);
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}
