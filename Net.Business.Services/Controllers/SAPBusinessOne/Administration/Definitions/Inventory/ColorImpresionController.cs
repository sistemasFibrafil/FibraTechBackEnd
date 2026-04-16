using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Administration.Definitions.Inventory
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ColorImpresionController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public ColorImpresionController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetList()
        {
            var objectGetList = await _repository.ColorImpresion.GetList();

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] ColorImpresionFindRequestDto value)
        {
            var objectGetList = await _repository.ColorImpresion.GetListByFiltro(value.ReturnValue());

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.dataList);
        }
    }
}
