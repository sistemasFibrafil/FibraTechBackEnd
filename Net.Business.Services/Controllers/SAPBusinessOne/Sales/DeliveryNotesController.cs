using System;
using Net.Data;
using System.Linq;
using FluentValidation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
using Net.Business.Services.Mappers.SAPBusinessOne;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Sales
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class DeliveryNotesController(IRepositoryWrapper repository, IValidator<DeliveryNotesCreateRequestDto> validatorCreate, IValidator<DeliveryNotesUpdateRequestDto> validatorUpdate) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<DeliveryNotesCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<DeliveryNotesUpdateRequestDto> _validatorUpdate = validatorUpdate;


        #region <<< CONSULTAS >>>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] DeliveryNotesFilterRequestDto value)
        {
            var result = await _repository.DeliveryNotes.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.dataList);
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetByDocEntry(int docEntry)
        {
            var result = await _repository.DeliveryNotes.GetByDocEntry(docEntry);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.data);
        }

        #endregion


        #region <<< OPERACIONES >>>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] DeliveryNotesCreateRequestDto dto)
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

            var entity = DeliveryNotesCreateMapper.ToEntity(dto);
            var result = await _repository.DeliveryNotes.SetCreate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] DeliveryNotesUpdateRequestDto dto)
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

            var entity = DeliveryNotesUpdateMapper.ToEntity(dto);
            var result = await _repository.DeliveryNotes.SetUpdate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetClose([FromBody] DeliveryNotesCloseRequestDto dto)
        {
            var entity = DeliveryNotesCloseMapper.ToEntity(dto);
            var result = await _repository.DeliveryNotes.SetClose(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetCancel([FromBody] DeliveryNotesCancelRequestDto dto)
        {
            var entity = DeliveryNotesCancelMapper.ToEntity(dto);
            var result = await _repository.DeliveryNotes.SetCancel(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        #endregion


        #region <<< IMPRESIONES >>>

        [HttpGet("{docEntry}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetPrintNationalDocEntry(int docEntry)
        {
            var objectGetById = await _repository.DeliveryNotes.GetPrintNationalDocEntry(docEntry);

            var nombreArchivo = string.Format("Entrega Nacional - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetPrintExportDocEntry(int docEntry)
        {
            var objectGetById = await _repository.DeliveryNotes.GetPrintExportDocEntry(docEntry);

            var nombreArchivo = string.Format("Entrega Exportacion - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        #endregion
    }
}
