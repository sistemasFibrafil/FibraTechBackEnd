using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Administration.Definitions.Financials
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TaxGroupsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public TaxGroupsController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] string filter)
        {
            var objectGetList = await _repository.TaxGroups.GetListByFilter(filter);

            if (objectGetList.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetList);
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetBySplCode([FromQuery] int slpCode)
        {
            var objectGet = await _repository.TaxGroups.GetBySplCode(slpCode);

            if (objectGet.ResultadoCodigo == -1)
            {
                return BadRequest(objectGet);
            }

            return Ok(objectGet.data);
        }
    }
}
