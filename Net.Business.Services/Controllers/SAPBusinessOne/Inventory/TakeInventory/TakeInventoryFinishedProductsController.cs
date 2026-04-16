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
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TakeInventoryFinishedProductsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public TakeInventoryFinishedProductsController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] TakeInventoryFinishedProductsFilterRequestDto value)
        {
            var result = await _repository.TakeInventoryFinishedProducts.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSummaryItemExcelByFilter([FromQuery] TakeInventoryFinishedProductsFilterRequestDto value)
        {
            try
            {
                var result = await _repository.TakeInventoryFinishedProducts.GetSummaryItemExcelByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetSummaryUserExcelByFilter([FromQuery] TakeInventoryFinishedProductsFilterRequestDto value)
        {
            try
            {
                var result = await _repository.TakeInventoryFinishedProducts.GetSummaryUserExcelByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetDetailedExcelByFilter([FromQuery] TakeInventoryFinishedProductsFilterRequestDto value)
        {
            try
            {
                var result = await _repository.TakeInventoryFinishedProducts.GetDetailedExcelByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetListByItemCode([FromQuery] TakeInventoryFinishedProductsModalFilterRequestDto value)
        {
            var result = await _repository.TakeInventoryFinishedProducts.GetListByItemCode(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListCurrentDate([FromQuery] TakeInventoryFinishedProductsFindRequestDto value)
        {
            var result = await _repository.TakeInventoryFinishedProducts.GetListCurrentDate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetToCopy([FromQuery] TakeInventoryFinishedProductsToCopyFindRequestDto value)
        {
            var result = await _repository.TakeInventoryFinishedProducts.GetToCopy(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] TakeInventoryFinishedProductsCreateRequestDto value)
        {
            var result = await _repository.TakeInventoryFinishedProducts.SetCreate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SetDeleteLine([FromBody] TakeInventoryFinishedProducts1DeleteRequestDto value)
        {
            var result = await _repository.TakeInventoryFinishedProducts.SetDeleteLine(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok();
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SetDelete([FromBody] TakeInventoryFinishedProductsDeleteRequestDto value)
        {
            var result = await _repository.TakeInventoryFinishedProducts.SetDelete(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok();
        }
    }
}
