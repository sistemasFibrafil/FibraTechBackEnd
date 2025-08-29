using System;
using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Web.Ventas
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PickingListController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public PickingListController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetListPickingPdfByDocEntry(int docEntry)
        {
            var objectGetById = await _repository.PickingList.GetListPickingPdfByDocEntry(docEntry);

            var nombreArchivo = string.Format("Picking List - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }
    }
}
