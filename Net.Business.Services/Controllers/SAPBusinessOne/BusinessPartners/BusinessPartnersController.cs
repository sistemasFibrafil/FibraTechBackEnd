using System;
using Net.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.SAPBusinessOne.BusinessPartners
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class BusinessPartnersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public BusinessPartnersController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] BusinessPartnersFilterRequestDto value)
        {
            var result = await _repository.BusinessPartners.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListModalByFilter([FromQuery] BusinessPartnersModalFilterRequestDto value)
        {
            var result = await _repository.BusinessPartners.GetListModalByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCode([FromQuery] string cardCode)
        {
            var result = await _repository.BusinessPartners.GetByCode(cardCode);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVehicleByCode([FromQuery] string cardCode)
        {
            var result = await _repository.BusinessPartners.GetVehicleByCode(cardCode);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.data);
        }

        [HttpGet] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDriverByCode([FromQuery] string cardCode)
        {
            var result = await _repository.BusinessPartners.GetDriverByCode(cardCode);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListClienteBySectorStatus([FromQuery] BusinessPartnersSectorStatusFilterRequestDto value)
        {
            var result = await _repository.BusinessPartners.GetListClienteBySectorStatus(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetClienteBySectorStatusExcel([FromQuery] BusinessPartnersSectorStatusFilterRequestDto value)
        {
            try
            {
                var result = await _repository.BusinessPartners.GetClienteBySectorStatusExcel(value.ReturnValue());

                result.data.Seek(0, SeekOrigin.Begin);
                var file = result.data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByRUC([FromQuery] string ruc)
        {
            var result = await _repository.BusinessPartners.GetByRUC(ruc);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] BusinessPartnersCreateRequestDto value)
        {
            var result = await _repository.BusinessPartners.SetCreate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] BusinessPartnersUpdateRequestDto value)
        {
            var result = await _repository.BusinessPartners.SetUpdate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromQuery] string cardCode)
        {
            var result = await _repository.BusinessPartners.SetDelete(cardCode);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
