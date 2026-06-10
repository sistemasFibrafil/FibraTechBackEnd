using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Administration.Definitions.Financials
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TaxGroupsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public TaxGroupsController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetList()
        {
            var resutl = await _repository.TaxGroups.GetList();

            if (resutl.ResultadoCodigo == -1)
            {
                return BadRequest(resutl);
            }

            return Ok(resutl.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] string filter)
        {
            var resutl = await _repository.TaxGroups.GetListByFilter(filter);
            
            if (resutl.ResultadoCodigo == -1)
            {
                return BadRequest(resutl);
            }

            return Ok(resutl.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetByCardCode([FromQuery] TaxGroupsFindRequestDto value)
        {
            var resutl = await _repository.TaxGroups.GetByCardCode(value.ReturnValue());

            if (resutl.ResultadoCodigo == -1)
            {
                return BadRequest(resutl);
            }

            return Ok(resutl.Data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCode([FromQuery] string code)
        {
            var resutl = await _repository.TaxGroups.GetByCode(code);

            if (resutl.ResultadoCodigo == -1)
            {
                return BadRequest(resutl);
            }

            return Ok(resutl.Data);
        }
    }
}
