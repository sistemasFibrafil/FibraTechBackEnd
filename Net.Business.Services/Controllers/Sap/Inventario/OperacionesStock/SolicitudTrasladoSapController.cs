using System;
using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Inventario.OperacionesStock
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SolicitudTrasladoSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public SolicitudTrasladoSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetSolicitudTrasladoPdfByDocEntry(int id)
        {
            var objectGetById = await _repository.SolicitudTrasladoSap.GetSolicitudTrasladoPdfByDocEntry(id);

            var nombreArchivo = string.Format("Solicitud de traslado - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }
    }
}
