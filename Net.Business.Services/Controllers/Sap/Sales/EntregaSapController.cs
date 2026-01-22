using Net.Data;
using Net.Business.DTO;
using Net.Business.DTO.Sap;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System;
namespace Net.Business.Services.Controllers.Sap.Sales
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class EntregaSapController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public EntregaSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListGuiaByFecha([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.Entrega.GetListGuiaByFecha(value.ReturnValue());

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
        public async Task<IActionResult> GetGuiaExcelByFecha([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.Entrega.GetGuiaExcelByFecha(value.ReturnValue());

                objectGetAll.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetAll.data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
