using System;
using Net.Data;
using Net.Business.DTO;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.IO;
namespace Net.Business.Services.Controllers.Web.Inventario.OperacionesStock
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LecturaController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public LecturaController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByBaseTypeAndBaseEntry([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.Lectura.GetListByBaseTypeAndBaseEntry(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.Lectura.GetListByFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListByBaseTypeBaseEntryBaseLineFiltro([FromBody] FilterRequestDto value)
        {
            var objectGetAll = await _repository.Lectura.GetListByBaseTypeBaseEntryBaseLineFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListByTargetTypeTrgetEntryTrgetLineFiltro([FromBody] FilterRequestDto value)
        {
            var objectGetAll = await _repository.Lectura.GetListByTargetTypeTrgetEntryTrgetLineFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] LecturaCreateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a crear ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var objectNew = await _repository.Lectura.SetCreate(value.ReturnValue());

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
        public async Task<IActionResult> SetDeleteMassive([FromBody] LecturaDeleteRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registro a eliminar ..!");
            }

            var response = await _repository.Lectura.SetDeleteMassive(value.ReturnValue());

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
        public async Task<IActionResult> SetDelete([FromBody] LecturaDeleteRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registro a eliminar ..!");
            }

            var response = await _repository.Lectura.SetDelete(value.ReturnValue());

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLecturaCopyToTransferencia([FromBody] LecturaCopyToTransferenciaFindDto value)
        {
            var objectGetAll = await _repository.Lectura.GetLecturaCopyToTransferencia(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.data);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetPackingListPdfByTargetTypeTrgetEntry([FromQuery] FilterRequestDto value)
        {
            var objectGetByTargetTypeTrgetEntry = await _repository.Lectura.GetPackingListPdfByTargetTypeTrgetEntry(value.ReturnValue());

            if (objectGetByTargetTypeTrgetEntry.IdRegistro == -1)
            {
                throw new FileNotFoundException(objectGetByTargetTypeTrgetEntry.ResultadoDescripcion);
            }

            var nombreArchivo = string.Format("Packing List - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetByTargetTypeTrgetEntry.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");
            return pdf;
        }
    }
}
