using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO;
using Net.Business.DTO.Sap;
using Net.Data;
namespace Net.Business.Services.Controllers.Sap.Inventario.SKU
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OSKCController : Controller
    {
        readonly IRepositoryWrapper _repository;
        public OSKCController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] OSKCCreateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a crear ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var obj = await _repository.OSKC.SetCreate(value.ReturnValue());

            if (obj.ResultadoCodigo == -1)
            {
                return BadRequest(obj);
            }

            return Ok(obj.data);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] OSKCUpdateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a crear ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var obj = await _repository.OSKC.SetUpdate(value.ReturnValue());

            if (obj.ResultadoCodigo == -1)
            {
                return BadRequest(obj);
            }

            return Ok(obj.data);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SetDelete([FromBody] OSKCDeleteRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registro a eliminar ..!");
            }

            var obj = await _repository.OSKC.SetDelete(value.ReturnValue());

            if (obj.ResultadoCodigo == -1)
            {
                return BadRequest(obj);
            }

            return Ok(obj.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByDateRange([FromQuery] OSKCFindByDateRequestDto value)
        {            
            var objectGetList = await _repository.OSKC.GetListByDateRange(value.ReturnValue());

            if (objectGetList == null)
            {
                return NotFound();
            }

            var obj = new OSKCGetListByDateRangeRequestDto().ReturnValue(objectGetList.dataList);

            return Ok(obj.GetList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCode([FromQuery] OSKCFindByCodeRequestDto value)
        {
            var objectGetList = await _repository.OSKC.GetByCode(value.ReturnValue());

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] OSKCFindByFiltroRequestDto value)
        {
            var objectGetList = await _repository.OSKC.GetListByFiltro(value.ReturnValue());

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOSKCExcel([FromQuery] OSKCFindByDateRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.OSKC.GetOSKCExcel(value.ReturnValue());

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
