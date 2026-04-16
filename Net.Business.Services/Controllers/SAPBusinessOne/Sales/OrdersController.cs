using System;
using Net.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using FluentValidation;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
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
    public class OrdersController
        (
            IRepositoryWrapper repository,
            IValidator<OrdersCreateRequestDto> validatorCreate,
            IValidator<OrdersUpdateRequestDto> validatorUpdate
        ) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<OrdersCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<OrdersUpdateRequestDto> _validatorUpdate = validatorUpdate;


        #region <<< CONSULTAS >>>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOpen()
        {
            var result = await _repository.Orders.GetListOpen();

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] OrdersFilterRequestDto value)
        {
            var result = await _repository.Orders.GetListByFilter(value.ReturnValue());

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
            var result = await _repository.Orders.GetByDocEntry(docEntry);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetToCopy(int docEntry)
        {
            var result = await _repository.Orders.GetToCopy(docEntry);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListSeguimientoByFilter([FromQuery] OrdersSeguimientoFindRequestDto value)
        {
            var objectGetList = await _repository.Orders.GetListSeguimientoByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetSeguimientoByFilterExcel([FromQuery] OrdersSeguimientoFindRequestDto value)
        {
            try
            {
                var objectGetFile = await _repository.Orders.GetSeguimientoByFilterExcel(value.ReturnValue());

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
        public async Task<IActionResult> GetListSeguimientoDetalladoDireccionFiscalByFilter([FromQuery] OrdersSeguimientoFindRequestDto value)
        {
            var objectGetList = await _repository.Orders.GetListSeguimientoDetalladoDireccionFiscalByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetSeguimientoDetalladoDireccionFiscalByFilterExcel([FromQuery] OrdersSeguimientoFindRequestDto value)
        {
            try
            {
                var objectGetFile = await _repository.Orders.GetSeguimientoDetalladoDireccionFiscalByFilterExcel(value.ReturnValue());

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
        public async Task<IActionResult> GetListSeguimientoDetalladoDireccionDespachoByFilter([FromQuery] OrdersSeguimientoFindRequestDto value)
        {
            var objectGetList = await _repository.Orders.GetListSeguimientoDetalladoDireccionDespachoByFilter(value.ReturnValue());

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
        public async Task<IActionResult> GetSeguimientoDetalladoDireccionDespachoByFilterExcel([FromQuery] OrdersSeguimientoFindRequestDto value)
        {
            try
            {
                var objectGetFile = await _repository.Orders.GetSeguimientoDetalladoDireccionDespachoByFilterExcel(value.ReturnValue());

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
            var objectGetList = await _repository.Orders.GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(value.ReturnValue());

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
                var objectGetFile = await _repository.Orders.GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha(value.ReturnValue());

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
            var objectGetList = await _repository.Orders.GetListOrdenVentaProgramacionByFecha(value.ReturnValue());

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
                var objectGetFile = await _repository.Orders.GetOrdenVentaProgramacionExcelByFecha(value.ReturnValue());

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
            var objectGetList = await _repository.Orders.GetListOrdenVentaSodimacPendienteByFiltro(value.ReturnValue());

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
            var objectGet = await _repository.Orders.GetOrdenVentaSodimacPendienteById(value.ReturnValue());

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
            var objectGetList = await _repository.Orders.GetListOrdenVentaPreliminarPendienteByFecha(value.ReturnValue());

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
                var objectGetFile = await _repository.Orders.GetListOrdenVentaPreliminarPendienteExcelByFecha(value.ReturnValue());

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromForm] string value, [FromForm] IList<IFormFile> files)
        {
            var dto = JsonConvert.DeserializeObject<OrdersCreateRequestDto>(value);

            // 🔥 Asignar archivos manualmente
            if (files != null && files.Any())
            {
                dto.Attachments2 ??= new Attachments2CreateDto();

                for (int i = 0; i < files.Count; i++)
                {
                    var f = files[i];
                    var line = dto.Attachments2.Lines[i];

                    var expectedName = $"{line.FileName}.{line.FileExt}";
                    var incomingName = Path.GetFileName(f.FileName);

                    // 🔥 Comparación segura
                    if (string.Equals(incomingName, expectedName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!Directory.Exists(line.TrgtPath))
                        {
                            return BadRequest($"La ruta no existe: {line.TrgtPath}");
                        }

                        var fullPath = Path.Combine(line.TrgtPath, expectedName);

                        try
                        {
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await f.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            return BadRequest($"Error al guardar archivo en ruta: {line.TrgtPath}. Detalle: {ex.Message}");
                        }
                    }
                }
            }

            var validationResult = await _validatorCreate.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                }));
            }

            var entity = OrdersCreateMapper.ToEntity(dto);
            var result = await _repository.Orders.SetCreate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] OrdersUpdateRequestDto dto)
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

            var entity = OrdersUpdateMapper.ToEntity(dto);
            var result = await _repository.Orders.SetUpdate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetClose([FromBody] OrdersCloseRequestDto value)
        {
            var result = await _repository.Orders.SetClose(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

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
            var objectGetById = await _repository.Orders.GetPrintNationalDocEntry(docEntry);

            var nombreArchivo = string.Format("Orden de compra nacional - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetPrintExportPlantaDocEntry(int docEntry)
        {
            var objectGetById = await _repository.Orders.GetPrintExportPlantaDocEntry(docEntry);

            var nombreArchivo = string.Format("Orden de compra exportacion - planta - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetPrintExportClienteDocEntry(int docEntry)
        {
            var objectGetById = await _repository.Orders.GetPrintExportClienteDocEntry(docEntry);

            var nombreArchivo = string.Format("Orden de compra exportacion - cliente - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        #endregion
    }
}
