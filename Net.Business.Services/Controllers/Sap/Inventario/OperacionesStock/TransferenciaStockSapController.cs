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
    public class TransferenciaStockSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public TransferenciaStockSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetTransferenciaStockPdfByDocEntry(int id)
        {
            var objectGetById = await _repository.TransferenciaStockSap.GetTransferenciaStockPdfByDocEntry(id);

            var nombreArchivo = string.Format("Transferencia de stock - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }
    }
}
