using System;
using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Sales;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Close;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Create;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Update;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Sales
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class DeliveryNotesController
        (
            IRepositoryWrapper repository,
            IDeliveryNotesService deliveryNotesService
        ) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IDeliveryNotesService _deliveryNotesService = deliveryNotesService;
       

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetCreate([FromBody] DeliveryNotesCreateRequestDto dto)
        {
            var result = await _deliveryNotesService.SetCreate(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetUpdate([FromBody] DeliveryNotesUpdateRequestDto dto)
        {
            var result = await _deliveryNotesService.SetUpdate(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetCancel([FromBody] DeliveryNotesCancelRequestDto dto)
        {
            var result = await _deliveryNotesService.SetCancel(dto);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetClose([FromBody] DeliveryNotesCloseRequestDto dto)
        {
            var result = await _deliveryNotesService.SetClose(dto);

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
