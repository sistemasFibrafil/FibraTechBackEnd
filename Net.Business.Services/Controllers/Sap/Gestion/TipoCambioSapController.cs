using Net.Data;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Gestion
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TipoCambioSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public TipoCambioSapController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByFechaCode([FromQuery] TipoCambioFindRequestDto value)
        {
            var objectGet = await _repository.TipoCambioSap.GetByFechaCode(value.ReturnValue());

            if (objectGet.ResultadoCodigo == -1)
            {
                return BadRequest(objectGet);
            }

            return Ok(objectGet.data);
        }
    }
}
