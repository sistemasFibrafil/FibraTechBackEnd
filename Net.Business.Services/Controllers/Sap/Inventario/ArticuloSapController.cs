using System;
using Net.Data;
using System.IO;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Net.Business.DTO.Sap.Inventario.Articulo;
namespace Net.Business.Services.Controllers.Sap.Inventario
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ArticuloSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public ArticuloSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetListByFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCode([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetByCode(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListStockGeneralByAlmacen([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetListStockGeneralByAlmacen(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockGeneralByAlmacenExcel([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.ArticuloSap.GetStockGeneralByAlmacenExcel(value.ReturnValue());

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
        public async Task<IActionResult> GetListStockGeneralDetalladoAlmacenByAlmacen([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetListStockGeneralDetalladoAlmacenByAlmacen(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockGeneralDetalladoAlmacenByAlmacenExcel([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.ArticuloSap.GetStockGeneralDetalladoAlmacenByAlmacenExcel(value.ReturnValue());

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
        public async Task<IActionResult> GetListArticuloVentaByGrupoSubGrupoEstado([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetListArticuloVentaByGrupoSubGrupoEstado(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticuloVentaExcelByGrupoSubGrupoEstado([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.ArticuloSap.GetArticuloVentaExcelByGrupoSubGrupoEstado(value.ReturnValue());

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
        public async Task<IActionResult> GetListArticuloVentaStockByGrupoSubGrupo([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetListArticuloVentaStockByGrupoSubGrupo(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticuloVentaStockExcelByGrupoSubGrupo([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.ArticuloSap.GetArticuloVentaStockExcelByGrupoSubGrupo(value.ReturnValue());

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
        public async Task<IActionResult> GetListArticuloByGrupoSubGrupoFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetListArticuloByGrupoSubGrupoFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListArticuloExcelByGrupoSubGrupoFiltro([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.ArticuloSap.GetListArticuloExcelByGrupoSubGrupoFiltro(value.ReturnValue());

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
        public async Task<IActionResult> GetListMovimientoStockByFechaSede([FromQuery] MovimientoStockSapByFechaSedeFindDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetListMovimientoStockByFechaSede(value.ReturnValue());

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
        public async Task<IActionResult> GetMovimientoStockExcelByFechaSede([FromQuery] MovimientoStockSapByFechaSedeFindDto value)
        {
            try
            {
                var objectGetAll = await _repository.ArticuloSap.GetMovimientoStockExcelByFechaSede(value.ReturnValue());

                objectGetAll.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetAll.data.ToArray();

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
        public async Task<IActionResult> GetArticuloForOrdenVentaSodimacBySku([FromBody] ArticuloSapForSodimacBySkuDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetArticuloForOrdenVentaSodimacBySku(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticuloVentaByCode([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.ArticuloSap.GetArticuloVentaByCode(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.data);
        }
    }
}
