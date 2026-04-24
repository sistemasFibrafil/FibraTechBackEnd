using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.BusinessPartners;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Filter;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.BusinessPartners.Vehicles.Filter;
namespace Net.Business.Services.Controllers.SAPBusinessOne.BusinessPartners
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class VehiclesController
        (
            IRepositoryWrapper repository,
            IVehiclesService vehiclesService
        ) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IVehiclesService _vehicles = vehiclesService;


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] VehiclesFilterRequestDto dto)
        {
            var entity = VehiclesFilterMapper.ToEntity(dto);
            var result = await _repository.Vehicles.GetListByFilter(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetCreate([FromBody] VehiclesCreateRequestDto dto)
        {
            var result = await _vehicles.SetCreate(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}


