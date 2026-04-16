using System;
using Net.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Inventory.TakeInventory
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TakeInventorySparePartsController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public TakeInventorySparePartsController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] TakeInventorySparePartsFilterRequestDto value)
        {
            var result = await _repository.TakeInventorySpareParts.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExcelByFilter([FromQuery] TakeInventorySparePartsFilterRequestDto value)
        {
            try
            {
                var result = await _repository.TakeInventorySpareParts.GetExcelByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetListCurrentDate([FromQuery] TakeInventorySparePartsFindRequestDto value)
        {
            var result = await _repository.TakeInventorySpareParts.GetListCurrentDate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] TakeInventorySparePartsCreateRequestDto value)
        {
            var result = await _repository.TakeInventorySpareParts.SetCreate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] TakeInventorySparePartsUpdateRequestDto value)
        {
            var result = await _repository.TakeInventorySpareParts.SetUpdate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetDelete([FromBody] TakeInventorySparePartsDeleteRequestDto value)
        {
            var result = await _repository.TakeInventorySpareParts.SetDelete(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }
    }
}
