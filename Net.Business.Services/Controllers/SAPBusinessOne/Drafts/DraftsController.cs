using System;
using Net.Data;
using Newtonsoft.Json;
using Net.CrossCotting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Net.Business.DTO.SAPBusinessOne.Drafts.Create;
using Net.Business.DTO.SAPBusinessOne.Drafts.Filter;
using Net.Business.DTO.SAPBusinessOne.Drafts.Update;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Draft;
using Net.Business.Logic.Mappers.SAPBusinessOne.Drafts.Filter;
using Net.Business.DTO.SAPBusinessOne.Drafts.CreateToDocument;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Drafts
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class DraftsController
        (
            IRepositoryWrapper repository,
            IDraftService draftService
        ) : ControllerBase
    {
        private readonly IDraftService _draftService = draftService;
        private readonly IRepositoryWrapper _repository = repository;


        #region <<< CONSULTAS >>>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListDraftsDocumentReport([FromBody] DraftsDocumentReportFilterRequestDto dto)
        {
            var entity = DraftsDocumentReportFilterMapper.ToEntity(dto);
            var result = await _repository.Drafts.GetListDraftsDocumentReport(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.DataList);
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetByDocEntry(int docEntry)
        {
            var result = await _repository.Drafts.GetByDocEntry(docEntry);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }


        [HttpGet("{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetStatusByDocEntry(int docEntry)
        {
            var result = await _repository.Drafts.GetStatusByDocEntry(docEntry);

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
        public async Task<IActionResult> SetCreate([FromForm] string value, [FromForm] IList<IFormFile> files)
        {
            var dto = JsonConvert.DeserializeObject<DraftsCreateRequestDto>(value);

            if (dto == null)
            {
                return BadRequest(ResponseHelper.Error<object>("Datos inválidos"));
            }

            var result = await _draftService.SetCreate(dto, files);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetSaveDraftToDocument([FromForm] string value, [FromForm] IList<IFormFile> files)
        {
            var dto = JsonConvert.DeserializeObject<DraftsCreateToDocumentRequestDto>(value);

            if (dto == null)
            {
                return BadRequest(ResponseHelper.Error<object>("Datos inválidos"));
            }

            var result = await _draftService.SetSaveDraftToDocument(dto, files);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetUpdate([FromForm] string value, [FromForm] IList<IFormFile> files)
        {
            var dto = JsonConvert.DeserializeObject<DraftsUpdateRequestDto>(value);

            if (dto == null)
            {
                return BadRequest(ResponseHelper.Error<object>("Datos inválidos"));
            }

            var result = await _draftService.SetUpdate(dto, files);

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
            var result = await _repository.Drafts.GetPrintNationalDocEntry(docEntry);

            var nombreArchivo = string.Format("Orden de compra nacional - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(result.Data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetPrintExportPlantaDocEntry(int docEntry)
        {
            var result = await _repository.Drafts.GetPrintExportPlantaDocEntry(docEntry);

            var nombreArchivo = string.Format("Orden de compra exportacion - planta - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(result.Data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetPrintExportClienteDocEntry(int docEntry)
        {
            var result = await _repository.Drafts.GetPrintExportClienteDocEntry(docEntry);

            var nombreArchivo = string.Format("Orden de compra exportacion - cliente - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(result.Data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        #endregion
    }
}
