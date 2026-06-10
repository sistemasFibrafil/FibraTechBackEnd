using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Net.Business.Logic.Interfaces.SAPBusinessOne.BusinessPartners;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Ubigeo.Filter;
namespace Net.Business.Services.Controllers.SAPBusinessOne.BusinessPartners
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UbigeoController
        (
            IUbigeoService ubigeoService
        ) : ControllerBase
    {
        private readonly IUbigeoService _ubigeoService = ubigeoService;


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] UbigeoFilterRequestDto dto)
        {
            var result = await _ubigeoService.GetListByFilter(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }
    }
}
