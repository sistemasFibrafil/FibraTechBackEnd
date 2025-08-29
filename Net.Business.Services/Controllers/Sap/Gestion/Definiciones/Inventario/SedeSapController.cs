using Net.Data;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Gestion.Definiciones.Inventario
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SedeSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public SedeSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.SedeSap.GetListByFiltro(value.ReturnValue());

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var objectGet = await _repository.SedeSap.GetById(id);

            if (objectGet == null)
            {
                return NotFound();
            }

            return Ok(objectGet.data);
        }
    }
}
