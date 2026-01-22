using System;
using Net.Data;
using Net.Business.DTO.Sap;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Inventory
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PickingController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public PickingController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] PickingFilterDto value)
        {
            var result = await _repository.Picking.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByBaseEntry([FromQuery] PickingFindDto value)
        {
            var result = await _repository.Picking.GetListByBaseEntry(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByBaseEntryBaseType([FromQuery] PickingFindDto value)
        {
            var result = await _repository.Picking.GetListByBaseEntryBaseType(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByTarget([FromQuery] PickingFindByTrgetRequestDto value)
        {
            var result = await _repository.Picking.GetListByTarget(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetToCopy([FromBody] PickingCopyToFindDto value)
        {
            var result = await _repository.Picking.GetToCopy(value.ReturnValue());

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
        public async Task<IActionResult> SetCreate([FromBody] PickingCreateDto value)
        {
            var result = await _repository.Picking.SetCreate(value.ReturnValue());

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
        public async Task<IActionResult> SetRelease([FromBody] PickingReleaseDto value)
        {
            var result = await _repository.Picking.SetRelease(value.ReturnValue());

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
        public async Task<IActionResult> SetDelete([FromBody] PickingDeleteDto value)
        {
            var result = await _repository.Picking.SetDelete(value.ReturnValue());

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
        public async Task<IActionResult> SetDeleteMassive([FromBody] PickingDeleteDto value)
        {
            var result = await _repository.Picking.SetDeleteMassive(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok();
        }


        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetPickingPrint([FromQuery] PickingPrintFindRequestDto value)
        {
            var result = await _repository.Picking.GetPickingPrint(value.ReturnValue());

            if (result.IdRegistro == -1)
            {
                return NotFound(result.ResultadoDescripcion);
            }

            var nombreArchivo = $"Packing List - {DateTime.Now:dd-MM-yyyy}.pdf";
            return File(result.data.ToArray(), "application/pdf", nombreArchivo);
        }
    }
}
