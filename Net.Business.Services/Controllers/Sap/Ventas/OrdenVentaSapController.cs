using System;
using Net.Data;
using System.IO;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Ventas
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrdenVentaSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public OrdenVentaSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListSeguimientoByFilter([FromQuery] OrdenVentaSeguimientoFindDto value)
        {
            var objectGetList = await _repository.OrdenVentaSap.GetListSeguimientoByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetSeguimientoByFilterExcel([FromQuery] OrdenVentaSeguimientoFindDto value)
        {
            try
            {
                var objectGetFile = await _repository.OrdenVentaSap.GetSeguimientoByFilterExcel(value.ReturnValue());

                objectGetFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.data.ToArray();

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
        public async Task<IActionResult> GetListSeguimientoDetalladoDireccionFiscalByFilter([FromQuery] OrdenVentaSeguimientoFindDto value)
        {
            var objectGetList = await _repository.OrdenVentaSap.GetListSeguimientoDetalladoDireccionFiscalByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetSeguimientoDetalladoDireccionFiscalByFilterExcel([FromQuery] OrdenVentaSeguimientoFindDto value)
        {
            try
            {
                var objectGetFile = await _repository.OrdenVentaSap.GetSeguimientoDetalladoDireccionFiscalByFilterExcel(value.ReturnValue());

                objectGetFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.data.ToArray();

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
        public async Task<IActionResult> GetListSeguimientoDetalladoDireccionDespachoByFilter([FromQuery] OrdenVentaSeguimientoFindDto value)
        {
            var objectGetList = await _repository.OrdenVentaSap.GetListSeguimientoDetalladoDireccionDespachoByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetSeguimientoDetalladoDireccionDespachoByFilterExcel([FromQuery] OrdenVentaSeguimientoFindDto value)
        {
            try
            {
                var objectGetFile = await _repository.OrdenVentaSap.GetSeguimientoDetalladoDireccionDespachoByFilterExcel(value.ReturnValue());

                objectGetFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.data.ToArray();

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
        public async Task<IActionResult> GetListOrdenVentaPendienteStockAlmacenProduccionByFecha([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.OrdenVentaSap.GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(value.ReturnValue());

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
        public async Task<IActionResult> GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetFile = await _repository.OrdenVentaSap.GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha(value.ReturnValue());

                objectGetFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.data.ToArray();

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
        public async Task<IActionResult> GetListOrdenVentaProgramacionByFecha([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.OrdenVentaSap.GetListOrdenVentaProgramacionByFecha(value.ReturnValue());

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
        public async Task<IActionResult> GetOrdenVentaProgramacionExcelByFecha([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetFile = await _repository.OrdenVentaSap.GetOrdenVentaProgramacionExcelByFecha(value.ReturnValue());

                objectGetFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.data.ToArray();

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
        public async Task<IActionResult> GetListOrdenVentaSodimacPendienteByFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.OrdenVentaSap.GetListOrdenVentaSodimacPendienteByFiltro(value.ReturnValue());

            if (objectGetList.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetList);
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetOrdenVentaSodimacPendienteById([FromQuery] FilterRequestDto value)
        {
            var objectGet = await _repository.OrdenVentaSap.GetOrdenVentaSodimacPendienteById(value.ReturnValue());

            if (objectGet.ResultadoCodigo == -1)
            {
                return BadRequest(objectGet);
            }

            return Ok(objectGet.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenVentaPreliminarPendienteByFecha([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.OrdenVentaSap.GetListOrdenVentaPreliminarPendienteByFecha(value.ReturnValue());

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
        public async Task<IActionResult> GetListOrdenVentaPreliminarPendienteExcelByFecha([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetFile = await _repository.OrdenVentaSap.GetListOrdenVentaPreliminarPendienteExcelByFecha(value.ReturnValue());

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
