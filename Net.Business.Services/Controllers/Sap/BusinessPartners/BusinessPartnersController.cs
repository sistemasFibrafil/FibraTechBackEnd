using System;
using Net.Data;
using System.IO;
using Net.Business.DTO.Sap;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.BusinessPartners
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BusinessPartnersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public BusinessPartnersController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] BusinessPartnersFilterRequestDto value)
        {
            var objectGetList = await _repository.SocioNegocio.GetListByFilter(value.ReturnValue());

            if (objectGetList.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetList);
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCode([FromQuery] string cardCode)
        {
            var result = await _repository.SocioNegocio.GetByCode(cardCode);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }        

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListClienteBySectorStatus([FromQuery] BusinessPartnersSectorStatusFilterRequestDto value)
        {
            var objectGetList = await _repository.SocioNegocio.GetListClienteBySectorStatus(value.ReturnValue());

            if (objectGetList.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetList);
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetClienteBySectorStatusExcel([FromQuery] BusinessPartnersSectorStatusFilterRequestDto value)
        {
            try
            {
                var objectGetFile = await _repository.SocioNegocio.GetClienteBySectorStatusExcel(value.ReturnValue());

                objectGetFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
