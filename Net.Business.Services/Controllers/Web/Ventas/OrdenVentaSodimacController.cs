using System;
using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Net.Business.DTO.Web;
using Net.Business.DTO;
using System.IO;
using Net.Business.DTO.Web.Ventas.Sop;
namespace Net.Business.Services.Controllers.Web.Ventas
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrdenVentaSodimacController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public OrdenVentaSodimacController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] OrdenVentaSodimacCreateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a crear ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var objectNew = await _repository.OrdenVentaSodimac.SetCreate(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] OrdenVentaSodimacUpdateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a actualizar ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var objectNew = await _repository.OrdenVentaSodimac.SetUpdate(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenVentaSodimacByFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacByFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrdenVentaSodimacById(int id)
        {
            var objectGetAll = await _repository.OrdenVentaSodimac.GetOrdenVentaSodimacById(id);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenVentaSodimacPendienteLpnByFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacPendienteLpnByFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenVentaSodimacDetallePendienteLpnByIdAndFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacDetallePendienteLpnByIdAndFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenVentaSodimacLpnByFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacLpnByFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenVentaSodimacDetalleById([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacDetalleById(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetLpnUpdate([FromBody] OrdenVentaSodimacLpnUpdateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a actualizar ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var response = await _repository.OrdenVentaSodimac.SetLpnUpdate(value.ReturnValue());

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetBarcodeLpnPdfById(int id)
        {
            try
            {
                var objectGetById = await _repository.OrdenVentaSodimac.GetBarcodeLpnPdfById(id);

                var nombreArchivo = string.Format("Lpn - {0} - {1}", id.ToString(), DateTime.Now.ToString("dd-MM-yyyy").ToString());

                var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

                return pdf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{ean}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetListBarcodeEanPdfByEan(string ean)
        {
            try
            {
                var objectGetById = await _repository.OrdenVentaSodimac.GetListBarcodeEanPdfByEan(ean);

                var nombreArchivo = string.Format("Ean - {0} - {1}", ean, DateTime.Now.ToString("dd-MM-yyyy").ToString());

                var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

                return pdf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenVentaSodimacByFechaNumero([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacByFechaNumero(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetListOrdenVentaSodimacExcelByFechaNumero([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacExcelByFechaNumero(value.ReturnValue());

                objectGetAll.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetAll.data.ToArray();

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
        public async Task<IActionResult> GetListOrdenVentaSodimacSelvaFechaNumero([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacSelvaFechaNumero(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetListOrdenVentaSodimacSelvaPdfByFechaNumero([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetById = await _repository.OrdenVentaSodimac.GetListOrdenVentaSodimacSelvaPdfByFechaNumero(value.ReturnValue());

                var nombreArchivo = string.Format("Sodimac Selva - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

                var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

                return pdf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
