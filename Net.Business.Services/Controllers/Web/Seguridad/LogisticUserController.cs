using Net.Data;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Web.Seguridad
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LogisticUserController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public LogisticUserController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet("{idUsuario}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int idUsuario)
        {

            var result = await _repository.LogisticUser.GetById(new LogisticUserFindRequestDto { IdUsuario = idUsuario }.ReturnValue());

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.data);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] LogisticUserCreateRequestDto value)
        {
            var result = await _repository.LogisticUser.SetCreate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }
    }
}
