using Net.Data;
using Net.Business.DTO.Sap;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Administration.SystemInitialization
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TipoDocumentoSunatController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public TipoDocumentoSunatController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByTipo([FromQuery] TipoDocumentoSunatFindRequestDto value)
        {
            var result = await _repository.TipoDocumentoSunat.GetListByTipo(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }
    }
}
