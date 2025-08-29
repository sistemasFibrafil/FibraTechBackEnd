using Net.Data;
using Net.Business.DTO;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Web.Gestion.Definiciones.Inventario
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SedeController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public SedeController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.Sede.GetListByFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetAction([FromBody] SedeActionRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a crear ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var objectNew = await _repository.Sede.SetAction(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }
    }
}
