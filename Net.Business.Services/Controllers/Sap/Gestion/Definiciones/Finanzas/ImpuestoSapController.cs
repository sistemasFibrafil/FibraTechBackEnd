using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Gestion.Definiciones.Finanzas
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ImpuestoSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public ImpuestoSapController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] string filter)
        {
            var objectGetList = await _repository.ImpuestoSap.GetListByFiltro(filter);

            if (objectGetList.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetList);
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetBySplCode([FromQuery] int slpCode)
        {
            var objectGet = await _repository.ImpuestoSap.GetBySplCode(slpCode);

            if (objectGet.ResultadoCodigo == -1)
            {
                return BadRequest(objectGet);
            }

            return Ok(objectGet.data);
        }
    }
}
