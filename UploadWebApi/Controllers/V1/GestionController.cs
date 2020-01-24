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
    [RoutePrefix("v1/gestion")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GestionController : BaseApiController
    {
        readonly GestionHuellasService _service;
        readonly IIdentityService _identity;

        public GestionController(GestionHuellasService service, IIdentityService identity)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _identity= identity ?? throw new ArgumentNullException(nameof(identity));
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(PaginatedList<GetRowHuellaDto>))]
        public async Task<IHttpActionResult> Get(int pageNumber = 1,int pageSize=10, OrdenType orden= OrdenType.DESC)
        {
            try
            {
                Tuple<IEnumerable<GetRowHuellaDto>, int> resul = null;

                RangoPaginacion rangoPaginacion = new RangoPaginacion(pageNumber, pageSize);

                //if (_identity.IsSysAdmin || _identity.Roles.Where(r => r == ROL_CONTRASTADOR).Any())
                //    resul = await _service.ConsultarHuellasAsync(rangoPaginacion, _identity.AppIdentity, orden);
                //else
                //resul = await _service.ConsultarHuellasAsync(rangoPaginacion, _identity.UserIdentity, _identity.AppIdentity, orden);

                resul = await _service.ConsultarHuellasAsync(rangoPaginacion, orden);

                var paginacion = new PaginatedList<GetRowHuellaDto>(resul.Item1, pageNumber, pageSize, resul.Item2);

                //Link de pagina anterior
                if (paginacion.HasPreviousPage)
                    paginacion.Links.PreviousPage = GetLinkNewPage(pageNumber - 1);


                //Link de pagina siguiente
                if (paginacion.HasNextPage)
                    paginacion.Links.NextPage = GetLinkNewPage(pageNumber + 1);



                return Ok(paginacion);
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

        [HttpGet]
        [Route("{idMuestra}",Name = "GetHuellaById")]
        [ResponseType(typeof(GetHuellaDto))]
        public async Task<IHttpActionResult> Get(string idMuestra)
        {
            try
            {
                var huella = await _service.ConsultarHuellaAsync(idMuestra);
                huella.LinkDescarga = GetLinkDescarga(huella.IdHuella);
                return Ok(huella);
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (IdNoEncontradaException exN)
            {
                return NotFound(exN.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        [HttpGet]
        [Route("{idMuestra}/download")]
        public async Task<IHttpActionResult> Download(string idMuestra)
        {
            try
            {
                BlobDto dataStream = await _service.DownloadHuellaAsync(idMuestra, _identity.AppIdentity);



                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(dataStream.FileStream)
                };
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = dataStream.NombreFichero
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return ResponseMessage(response);
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (IdNoEncontradaException noEx)
            {
                return NotFound(noEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        [HttpGet]
        [Route("{idMuestra}/{idHuella:int}/download")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Download(string idMuestra,int idHuella)
        {
            try
            {
                BlobDto dataStream = await _service.DownloadHuellaAsync(idHuella);



                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(dataStream.FileStream)
                };
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = dataStream.NombreFichero
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return ResponseMessage(response);
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (IdNoEncontradaException noEx)
            {
                return NotFound(noEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        [Route("")]
        [HttpPost]
        [ResponseType(typeof(GetHuellaDto))]
        [ValidateModel]
        public async Task<IHttpActionResult> Post([FromBody] InsertHuellaDto dto)
        {

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.MensajesError();
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, String.Join(",", errorModel.ToArray()));
                return ResponseMessage(response);
            }

            try
            {

                await _service.VerificarFichero(dto.FileStream,dto.Hash);

                var inserted = await _service.CrearRegistroHuellaAsync(dto, _identity.UserIdentity, _identity.AppIdentity);

                string uri = Url.Link("GetHuellaById", new { idMuestra = dto.IdMuestra });

                return Created(uri, inserted);
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
        
        [HttpDelete]
        [Route("{idMuestra}")]
        public async Task<IHttpActionResult> Delete(string idMuestra)
        {
       
            try
            {

                if (_identity.IsSysAdmin)
                {
                    await _service.BorrarRegistroHuellaAsync(idMuestra, Guid.Empty, _identity.AppIdentity, true);
                }
                else
                {
                    await _service.BorrarRegistroHuellaAsync(idMuestra, _identity.UserIdentity, _identity.AppIdentity, false);
                }

                return NoContent();
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (IdNoEncontradaException noEx)
            {
                return NotFound(noEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    
    }
}
