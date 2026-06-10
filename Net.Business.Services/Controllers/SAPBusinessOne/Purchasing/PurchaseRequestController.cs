using System;
using Net.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Purchasing;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Purchasing
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PurchaseRequestController
        (
            IRepositoryWrapper repository,
            IPurchaseRequestService purchaseRequestService
        ) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IPurchaseRequestService _purchaseRequestService = purchaseRequestService;


        #region <<< CONSULTAS >>>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] PurchaseRequestFilterRequestDto value)
        {
            var result = await _repository.PurchaseRequest.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        
        [HttpGet("{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetByDocEntry(int docEntry)
        {
            var result = await _repository.PurchaseRequest.GetByDocEntry(docEntry);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        #endregion


        #region <<< OPERACIONES >>>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetValidateLinesItemsExcel([FromBody] List<PurchaseRequestLinesItemsValidateRequestDto> dto)
        {
            var result = await _purchaseRequestService.SetValidateLinesItemsExcel(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetValidateLinesServicesExcel([FromBody] List<PurchaseRequestLinesServicesValidateRequestDto> dto)
        {
            var result = await _purchaseRequestService.SetValidateLinesServicesExcel(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.DataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetCreate([FromBody] PurchaseRequestCreateRequestDto dto)
        {
            var result = await _purchaseRequestService.SetCreate(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetUpdate([FromBody] PurchaseRequestUpdateRequestDto dto)
        {
            var result = await _purchaseRequestService.SetUpdate(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetClose([FromBody] PurchaseRequestCloseRequestDto dto)
        {
            var result = await _purchaseRequestService.SetClose(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion


        #region <<< EXPORTACIONES >>>

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetDownloadItemsTemplate()
        {
            try
            {
                var objectGetFile = await _repository.PurchaseRequest.GetDownloadItemsTemplate();

                objectGetFile.Data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.Data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetDownloadServicesTemplate()
        {
            try
            {
                var objectGetFile = await _repository.PurchaseRequest.GetDownloadServicesTemplate();

                objectGetFile.Data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.Data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        #endregion
    }
}
