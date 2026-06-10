using System;
using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Inventory.InventoryTransactions;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Update;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Inventory.InventoryTransactions
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class StockTransfersController
        (
            IRepositoryWrapper repository,
            IStockTransfersService stockTransfersService
        ) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IStockTransfersService _stockTransfersService = stockTransfersService;


        #region <<< CONSULTAS >>>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] StockTransfersFilterRequestDto value)
        {
            var result = await _repository.StockTransfers.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.DataList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetByDocEntry(int id)
        {
            var result = await _repository.StockTransfers.GetByDocEntry(id);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.Data);
        }

        #endregion


        #region <<< OPERACIONES >>>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetCreate([FromBody] StockTransfersCreateRequestDto dto)
        {
            var result = await _stockTransfersService.SetCreate(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetUpdate([FromBody] StockTransfersUpdateRequestDto dto)
        {
            var result = await _stockTransfersService.SetUpdate(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion


        #region <<< IMPRESIONES >>>

        [HttpGet("{docEntry}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetFormatoPdfByDocEntry(int docEntry)
        {
            var objectGetById = await _repository.StockTransfers.GetFormatoPdfByDocEntry(docEntry);

            var nombreArchivo = string.Format("Transferencia de stock - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.Data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        #endregion
    }
}
