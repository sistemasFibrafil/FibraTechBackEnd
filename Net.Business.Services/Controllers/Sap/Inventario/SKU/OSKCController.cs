using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO.Sap;
using Net.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Net.Business.Services.Controllers.Sap.Inventario.SKU
{
    /// <summary>
    /// Controlador para la Configuración de SKU de SAP (@FIB_OSKC)
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OSKCController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;

        public OSKCController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Crear una nueva configuración de SKU.
        /// </summary>
        /// <param name="value">Datos de la nueva configuración.</param>
        /// <returns>El resultado de la transacción.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] OSKCCreateRequestDto value)
        {
            var result = await _repository.OSKC.SetCreate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            // Retorna 201 Created con la ubicación del nuevo recurso
            return Ok(result);
        }

        /// <summary>
        /// Actualizar una configuración de SKU existente.
        /// </summary>
        /// <param name="value">Datos para actualizar.</param>
        /// <returns>Sin contenido si la operación es exitosa.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetUpdate([FromBody] OSKCUpdateRequestDto value)
        {
            var result = await _repository.OSKC.SetUpdate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        /// <summary>
        /// Eliminar una configuración de SKU.
        /// </summary>
        /// <param name="value">Datos del registro a eliminar.</param>
        /// <returns>Sin contenido si la operación es exitosa.</returns>
        [HttpDelete] // Verbo HTTP corregido
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetDelete([FromBody] OSKCDeleteRequestDto value)
        {
            var result = await _repository.OSKC.SetDelete(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        /// <summary>
        /// Obtener lista de configuraciones de SKU por rango de fechas.
        /// </summary>
        /// <param name="value">DTO con fecha de inicio y fin.</param>
        /// <returns>Lista de configuraciones.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByDateRange([FromQuery] OSKCFindByDateRequestDto value)
        {
            var result = await _repository.OSKC.GetListByDateRange(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            // Lógica simplificada para devolver directamente los datos
            return Ok(result.dataList);
        }

        /// <summary>
        /// Obtener una configuración de SKU por su código.
        /// </summary>
        /// <param name="value">DTO con el código a buscar.</param>
        /// <returns>La configuración de SKU encontrada.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCode([FromQuery] OSKCFindByCodeRequestDto value)
        {
            var result = await _repository.OSKC.GetByCode(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.data);
        }

        /// <summary>
        /// Obtener lista de configuraciones de SKU por un filtro de texto.
        /// </summary>
        /// <param name="value">DTO con el texto de filtro.</param>
        /// <returns>Lista de configuraciones.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFiltro([FromQuery] OSKCFindByFiltroRequestDto value)
        {
            var result = await _repository.OSKC.GetListByFiltro(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return NotFound(result);
            }

            return Ok(result.dataList);
        }

        /// <summary>
        /// Generar y descargar un reporte de configuraciones de SKU en formato Excel.
        /// </summary>
        /// <param name="value">DTO con el rango de fechas para el reporte.</param>
        /// <returns>Un archivo de Excel.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOSKCExcel([FromQuery] OSKCFindByDateRequestDto value)
        {
            try
            {
                var result = await _repository.OSKC.GetOSKCExcel(value.ReturnValue());

                if (result.ResultadoCodigo == -1)
                {
                    return BadRequest(result);
                }

                result.data.Seek(0, SeekOrigin.Begin);
                var file = result.data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                // Retornar un error 500 en caso de una excepción inesperada
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}