using System;
using Net.Data;
using System.IO;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
using Net.Business.DTO.SAPBusinessOne.Inventory.Items.Update;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Inventory;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Inventory
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class ItemsController
        (
            IRepositoryWrapper repository,
            IItemsService itemsService
        ) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IItemsService _itemsService = itemsService;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] ItemsFilterRequestDto value)
        {
            var result = await _repository.Items.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByCode([FromQuery] ItemsFindByCodeRequestDto value)
        {
            var result = await _repository.Items.GetListByCode(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListStockGeneralSummary([FromQuery] ItemsStockGeneralViewFilterRequestDto value)
        {
            var result = await _repository.Items.GetListStockGeneralSummary(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockGeneralSummaryExcel([FromQuery] ItemsStockGeneralViewFilterRequestDto value)
        {
            try
            {
                var result = await _repository.Items.GetStockGeneralSummaryExcel(value.ReturnValue());

                result.Data.Seek(0, SeekOrigin.Begin);
                var file = result.Data.ToArray();

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
        public async Task<IActionResult> GetListStockGeneralDetailed([FromQuery] ItemsStockGeneralViewFilterRequestDto value)
        {
            var result = await _repository.Items.GetListStockGeneralDetailed(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockGeneralDetailedExcel([FromQuery] ItemsStockGeneralViewFilterRequestDto value)
        {
            try
            {
                var result = await _repository.Items.GetStockGeneralDetailedExcel(value.ReturnValue());

                result.Data.Seek(0, SeekOrigin.Begin);
                var file = result.Data.ToArray();

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
            var result = await _repository.Items.GetListArticuloVentaByGrupoSubGrupoEstado(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticuloVentaExcelByGrupoSubGrupoEstado([FromQuery] FilterRequestDto value)
        {
            try
            {
                var result = await _repository.Items.GetArticuloVentaExcelByGrupoSubGrupoEstado(value.ReturnValue());

                result.Data.Seek(0, SeekOrigin.Begin);
                var file = result.Data.ToArray();

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
            var result = await _repository.Items.GetListArticuloVentaStockByGrupoSubGrupo(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticuloVentaStockExcelByGrupoSubGrupo([FromQuery] FilterRequestDto value)
        {
            try
            {
                var result = await _repository.Items.GetArticuloVentaStockExcelByGrupoSubGrupo(value.ReturnValue());

                result.Data.Seek(0, SeekOrigin.Begin);
                var file = result.Data.ToArray();

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
            var result = await _repository.Items.GetListArticuloByGrupoSubGrupoFiltro(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListArticuloExcelByGrupoSubGrupoFiltro([FromQuery] FilterRequestDto value)
        {
            try
            {
                var result = await _repository.Items.GetListArticuloExcelByGrupoSubGrupoFiltro(value.ReturnValue());

                result.Data.Seek(0, SeekOrigin.Begin);
                var file = result.Data.ToArray();

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
        public async Task<IActionResult> GetListMovimientoStockByFechaSede([FromQuery] ItemsMovimientoStockFindRequestDto value)
        {
            var result = await _repository.Items.GetListMovimientoStockByFechaSede(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetMovimientoStockExcelByFechaSede([FromQuery] ItemsMovimientoStockFindRequestDto value)
        {
            try
            {
                var result = await _repository.Items.GetMovimientoStockExcelByFechaSede(value.ReturnValue());

                result.Data.Seek(0, SeekOrigin.Begin);
                var file = result.Data.ToArray();

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
        public async Task<IActionResult> GetArticuloForOrdenVentaSodimacBySku([FromBody] ItemsSodimacBySkuFindRequestDto value)
        {
            var result = await _repository.Items.GetArticuloForOrdenVentaSodimacBySku(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticuloVentaByCode([FromQuery] FilterRequestDto value)
        {
            var result = await _repository.Items.GetArticuloVentaByCode(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreateMassive([FromBody] ItemsCreateMassiveRequestDto value)
        {
            var result = await _repository.Items.SetCreateMassive(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetUpdateMassive([FromBody] List<ItemsUpdateMassiveRequestDto> dto)
        {
            var result = await _itemsService.SetUpdateMassive(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
