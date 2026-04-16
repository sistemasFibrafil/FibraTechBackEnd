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
    public class DocumentTypeSunatController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public DocumentTypeSunatController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByType([FromQuery] DocumentTypeSunatFindRequestDto value)
        {
           var result = await _repository.DocumentTypeSunat.GetListByType(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }
    }
}
