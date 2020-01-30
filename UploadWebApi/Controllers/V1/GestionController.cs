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
using FundacionOlivar.Modelos.ModelView;
using UploadWebApi.Aplicacion.Excepciones;
using UploadWebApi.Aplicacion.Servicios;
using UploadWebApi.Aplicacion.Stores;
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
        readonly IGestionHuellasService _service;

        public GestionController(IGestionHuellasService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(PaginatedList<GetRowHuellaDto>))]
        public async Task<IHttpActionResult> Get(int pageNumber = 1,int pageSize=10, OrdenType orden= OrdenType.DESC)
        {
            try
            {
                IQueryResult<GetRowHuellaDto> result = null;

                RangoPaginacion rangoPaginacion = new RangoPaginacion(pageNumber, pageSize);

                result = await _service.ConsultarHuellasAsync(rangoPaginacion, orden);



                var paginacion =result.AsPaginated(pageNumber, pageSize, GetLinkNewPage,(h) =>
                 {
                     h.LinkDetalle = GetLinkDetalle(h.IdHuella, h.IdMuestra);
                     h.LinkDescarga = GetLinkDescarga(h.IdHuella, h.IdMuestra);
                     return h;
                 });





                //var dtos = resul.Item1.Select(h => 
                //{
                //    h.LinkDetalle= GetLinkDetalle(h.IdHuella, h.IdMuestra);
                //    h.LinkDescarga = GetLinkDescarga(h.IdHuella, h.IdMuestra);
                //    return h;
                //});

                //var paginacion = new PaginatedList<GetRowHuellaDto>(dtos, pageNumber, pageSize, resul.Item2);

                ////Link de pagina anterior
                //if (paginacion.HasPreviousPage)
                //    paginacion.Links.PreviousPage = GetLinkNewPage(pageNumber - 1);


                ////Link de pagina siguiente
                //if (paginacion.HasNextPage)
                //    paginacion.Links.NextPage = GetLinkNewPage(pageNumber + 1);
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
                huella.LinkDescarga = GetLinkDescarga(huella.IdHuella, huella.IdMuestra);
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
        
        //[HttpGet]
        //[Route("{idMuestra}/download")]
        //public async Task<IHttpActionResult> Download(string idMuestra)
        //{
        //    try
        //    {
        //        BlobDto dataStream = await _service.DownloadHuellaAsync(idMuestra);



        //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StreamContent(dataStream.FileStream)
        //        };
        //        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = dataStream.NombreFichero
        //        };
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        //        return ResponseMessage(response);
        //    }
        //    catch (ServiceException sEx)
        //    {
        //        return BadRequest(sEx.Message);
        //    }
        //    catch (IdNoEncontradaException noEx)
        //    {
        //        return NotFound(noEx.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}
        
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

                var inserted = await _service.CrearRegistroHuellaAsync(dto);

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
        
        [HttpDelete]
        [Route("{idMuestra}")]
        public async Task<IHttpActionResult> Delete(string idMuestra)
        {
       
            try
            {

                await _service.BorrarRegistroHuellaAsync(idMuestra);

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
