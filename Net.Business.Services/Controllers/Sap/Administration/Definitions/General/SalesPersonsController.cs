using Net.Data;
using Net.Business.DTO.Sap;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Administration.Definitions.General
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SalesPersonsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public SalesPersonsController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetList()
        {
            var result = await _repository.SalesPersons.GetList();

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.SalesPersons.GetById(id);

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] SalesPersonsDto value)
        {
            var result = await _repository.SalesPersons.GetListByFiltro(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }
    }
}
