using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Inventory.SKU
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OSKPController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;

        public OSKPController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] OSKPCreateRequestDto value)
        {
            var result = await _repository.OSKP.SetCreate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] OSKPUpdateRequestDto value)
        {
            var result = await _repository.OSKP.SetUpdate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SetDelete([FromBody] OSKPDeleteRequestDto value)
        {
            var result = await _repository.OSKP.SetDelete(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] OSKPFindByFiltroRequestDto value)
        {
            var result = await _repository.OSKP.GetListByFiltro(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByDocEntry([FromQuery] OSKPFindByDocEntryRequestDto value)
        {
            var result = await _repository.OSKP.GetByDocEntry(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.data);
        }
    }
}
