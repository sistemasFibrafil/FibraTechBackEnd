using System;
using Net.Data;
using System.IO;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Sales
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class FacturaVentaSapController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public FacturaVentaSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListVentaProyeccionByFecha([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.FacturaVenta.GetListVentaProyeccionByFecha(value.ReturnValue());

            if (objectGetList.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetList);
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListVentaResumenByFechaGrupo([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.FacturaVenta.GetListVentaResumenByFechaGrupo(value.ReturnValue());

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
        public async Task<IActionResult> GetVentaResumenExcelByFechaGrupo([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectGetAll = await _repository.FacturaVenta.GetVentaResumenExcelByFechaGrupo(value.ReturnValue());

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
        public async Task<IActionResult> GetListVentaByFilter([FromQuery] VentaSapByFilterFindDto value)
        {
            var objectGetAll = await _repository.FacturaVenta.GetListVentaByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetVentaByFilterExcel([FromQuery] VentaSapByFilterFindDto value)
        {
            try
            {
                var objectGetAll = await _repository.FacturaVenta.GetVentaByFilterExcel(value.ReturnValue());

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
        public async Task<IActionResult> GetListFacturaVentaByFilter([FromQuery] FacturaVentaSapByFilterFindDto value)
        {
            var objectGetAll = await _repository.FacturaVenta.GetListFacturaVentaByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetFacturaVentaByFilterExcel([FromQuery] FacturaVentaSapByFilterFindDto value)
        {
            try
            {
                var objectGetAll = await _repository.FacturaVenta.GetFacturaVentaByFilterExcel(value.ReturnValue());

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
