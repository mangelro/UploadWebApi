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
using UploadWebApi.Applicacion.Servicios;
using UploadWebApi.Infraestructura.Servicios;
using UploadWebApi.Models;

namespace UploadWebApi.Controllers.V1
{

    [Authorize]
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
        [ResponseType(typeof(PaginatedList<GetHuellaDto>))]
        public async Task<IHttpActionResult> Get(int pageNumber = 1,int pageSize=10,string orden="desc")
        {
            try
            {
                Tuple<IEnumerable<GetHuellaDto>, int> resul = null;

                if (_identity.Roles.Where(r => r == "gestorcontrastes").Any())
                    resul = await _service.ConsultarHuellasAsync(pageNumber, pageSize, _identity.AppIdentity, orden);
                else
                    resul = await _service.ConsultarHuellasAsync(pageNumber, pageSize, _identity.UserIdentity, _identity.AppIdentity, orden);

                return Ok(new PaginatedList<GetHuellaDto>(CurrentUrl(), resul.Item1,pageNumber,pageSize,resul.Item2));
               
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
        [Route("{idMuestra}")]
        [ResponseType(typeof(GetHuellaDto))]
        public async Task<IHttpActionResult> Get(string idMuestra)
        {
            try
            {
                var huella = await _service.ConsultarHuellaAsync(idMuestra, _identity.AppIdentity);

                return Ok(huella);
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (NotFoundException noEx)
            {
                return BadRequest(noEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("{idMuestra}/download")]
        public async Task<IHttpActionResult> Descarga(string idMuestra)
        {
            try
            {
                BlobDto dataStream = await _service.DownloadHuellaAsync(idMuestra, _identity.AppIdentity);



                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(dataStream.Raw)
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
            catch (NotFoundException noEx)
            {
                return BadRequest(noEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] InsertHuellaDto dto)
        {
            try
            {
                var inserted = await _service.CrearRegistroHuellaAsync(dto, _identity.UserIdentity, _identity.AppIdentity);

                return Created("v1/gestion/" + inserted.IdMuestra, inserted);
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
                await _service.BorrarRegistroHuellaAsync(idMuestra,_identity.UserIdentity, _identity.AppIdentity);
                return NoContent();
            }
            catch (ServiceException sEx)
            {
                return BadRequest(sEx.Message);
            }
            catch (NotFoundException noEx)
            {
                return BadRequest(noEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string CurrentUrl() => Request.RequestUri.ToString().Split('?')[0];
        


    }
    
}
