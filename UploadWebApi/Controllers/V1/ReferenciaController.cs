using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

using FundacionOlivar.Web.Modelos;

using UploadWebApi.Aplicacion.Excepciones;
using UploadWebApi.Aplicacion.Servicios;
using UploadWebApi.Infraestructura.Datos.Excepciones;
using UploadWebApi.Infraestructura.Extensiones;
using UploadWebApi.Infraestructura.Filtros;
using UploadWebApi.Models;

namespace UploadWebApi.Controllers.V1
{

    [Authorize(Roles = "administradores,laboratorios,contrastadores")]
    //[Authorize]
    [RoutePrefix("v1/referencia")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReferenciaController : BaseApiController
    {
        readonly IVectorReferenciaService _service;

        public ReferenciaController(IVectorReferenciaService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }




        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] InsertVectorReferencia dto)
        {

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.MensajesError();
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, String.Join(",", errorModel.ToArray()));
                return ResponseMessage(response);
            }

            try
            {
                var inserted = await _service.CrearRegistroHuellaRefenciaAsync(dto);

                inserted.LinkDescarga = GetLinkDescarga(inserted.IdHuella, inserted.IdMuestra);
                return Created(GetLinkDescarga(inserted.IdHuella, inserted.IdMuestra), inserted);
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (MuestraDuplicadaException sEx)
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
