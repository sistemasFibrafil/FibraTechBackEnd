using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Administration.SystemInitialization;
using Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Find;
using Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Find;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Administration.SystemInitialization
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class DocumentSeriesConfigurationController
        (
            IRepositoryWrapper repository,
            IDocumentSeriesConfigurationService documentSeriesConfigurationService
        ) : Controller
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IDocumentSeriesConfigurationService _documentSeriesConfigurationService = documentSeriesConfigurationService;


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromQuery] DocumentSeriesConfigurationFindRequestDto dto)
        {
            var entity = DocumentSeriesConfigurationFindMapper.ToEntity(dto);
            var result = await _repository.DocumentSeriesConfiguration.GetById(entity);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetCreate([FromBody] DocumentSeriesConfigurationCreateRequestDto dto)
        {
            var result = await _documentSeriesConfigurationService.SetCreate(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
