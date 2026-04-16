using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Administration.SystemInitialization
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DocumentNumberingSeriesSunatController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public DocumentNumberingSeriesSunatController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListSerieDocumento([FromQuery] DocumentNumberingSeriesSunatFilterRequestDto value)
        {
            var result = await _repository.DocumentNumberingSeriesSunat.GetListSerieDocumento(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNumeroDocumentoByTipoSerie([FromQuery] DocumentNumberingSeriesSunatFindRequestDto value)
        {
            var result = await _repository.DocumentNumberingSeriesSunat.GetNumeroDocumentoByTipoSerie(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }
    }
}
