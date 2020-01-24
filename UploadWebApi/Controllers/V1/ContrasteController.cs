using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

using UploadWebApi.Aplicacion.Excepciones;
using UploadWebApi.Aplicacion.Servicios;
using UploadWebApi.Infraestructura.Datos.Excepciones;
using UploadWebApi.Models;

namespace UploadWebApi.Controllers.V1
{
    [Authorize(Roles = "administradores,contrastadores")]
    [RoutePrefix("v1/contrastar")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContrasteController : BaseApiController
    {
        readonly IContrasteHuellasService _service;



        public ContrasteController(IContrasteHuellasService  service)
        {
            _service= service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(ContrasteDto))]
        public async Task<IHttpActionResult> Get(string idMuestra1, string idMuestra2)
        {
            try
            {
                var contraste = await _service.ConstrastarHuellas(idMuestra1, idMuestra2);
                return Ok(contraste);
            }
            catch (IdNoEncontradaException noEx)
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
