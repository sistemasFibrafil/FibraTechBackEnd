using Net.Data;
using Net.Business.DTO;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.Web.Ventas.Sop;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System;
namespace Net.Business.Services.Controllers.Web.Ventas
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SopController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public SopController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.Sop.GetListByFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _repository.Sop.GetById(id);

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return Ok(response.data);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetSopExcelById([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetFile = await _repository.Sop.GetSopExcelById(value.ReturnValue());

                objectGetFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] SopCreateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a crear ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var objectNew = await _repository.Sop.SetCreate(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] SopUpdateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a actualizar ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var objectNew = await _repository.Sop.SetUpdate(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SetDelete([FromBody] SopDeleteRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registro a eliminar ..!");
            }

            var response = await _repository.Sop.SetDelete(value.ReturnValue());

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return Ok();
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SetDeleteDetalle([FromBody] SopDetalleDeleteRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registro a eliminar ..!");
            }

            var response = await _repository.Sop.SetDeleteDetalle(value.ReturnValue());

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return Ok();
        }
    }
}
