using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Net.Business.DTO.SAPBusinessOne.Administration.Definitions.General.Users.Find;
using Net.Business.DTO.SAPBusinessOne.Administration.Definitions.General.Users.Filter;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Administration.Definitions.General.Users.Find;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Administration.Definitions.General.Users.Filter;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Administration.Definitions.General
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UsersController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public UsersController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetList()
        {
            var result = await _repository.Users.GetList();

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] UsersFilterRequestDto dto)
        {
            var entity = UsersFilterMapper.ToEntity(dto);
            var result = await _repository.Users.GetListByFilter(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCode([FromQuery] UsersFindRequestDto dto)
        {
            var entity = UsersFindMapper.ToEntity(dto);
            var result = await _repository.Users.GetByCode(entity);

            if (result.ResultadoCodigo == -1)
                return NotFound(result);

            return Ok(result.data);
        }
    }
}
