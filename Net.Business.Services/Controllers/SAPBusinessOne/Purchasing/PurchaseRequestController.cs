using System;
using Net.Data;
using System.IO;
using System.Linq;
using FluentValidation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
using Net.Business.Services.Mappers.SAPBusinessOne;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Purchasing
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PurchaseRequestController(IRepositoryWrapper repository, IValidator<PurchaseRequestCreateRequestDto> validatorCreate, IValidator<PurchaseRequestUpdateRequestDto> validatorUpdate) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<PurchaseRequestCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<PurchaseRequestUpdateRequestDto> _validatorUpdate = validatorUpdate;


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

            return Ok(result.dataList);
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

            return Ok(result.data);
        }

        #endregion


        #region <<< EXPORTACIONES >>>

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetDownloadFormat()
        {
            try
            {
                var objectGetFile = await _repository.PurchaseRequest.GetDownloadFormat();

                objectGetFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectGetFile.data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        #endregion


        #region <<< OPERACIONES >>>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] PurchaseRequestCreateRequestDto dto)
        {
            var validationResult = await _validatorCreate.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                }));
            }

            var entity = PurchaseRequestCreateMapper.ToEntity(dto);
            var result = await _repository.PurchaseRequest.SetCreate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] PurchaseRequestUpdateRequestDto dto)
        {
            var validationResult = await _validatorUpdate.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                }));
            }

            var entity = PurchaseRequestUpdateMapper.ToEntity(dto);
            var result = await _repository.PurchaseRequest.SetUpdate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetClose([FromBody] PurchaseRequestCloseRequestDto value)
        {
            var result = await _repository.PurchaseRequest.SetClose(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        #endregion
    }
}
