using Net.Data;
using Net.Business.DTO.Sap;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Administration.Definitions.Inventory
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TipoLaminadoController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public TipoLaminadoController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetList()
        {
            var objectGetList = await _repository.TipoLaminado.GetList();

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] TipoLaminadoFinRequestDto value)
        {
            var objectGetList = await _repository.TipoLaminado.GetListByFiltro(value.ReturnValue());

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.dataList);
        }
    }
}
