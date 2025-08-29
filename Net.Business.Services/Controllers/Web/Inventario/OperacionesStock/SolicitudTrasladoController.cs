using Net.Data;
using Net.Business.DTO;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Web.Inventario.OperacionesStock
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SolicitudTrasladoController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public SolicitudTrasladoController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.SolicitudTraslado.GetListByFiltro(value.ReturnValue());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _repository.SolicitudTraslado.GetById(id);

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return Ok(response.data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetListById(int id)
        {
            var response = await _repository.SolicitudTraslado.GetListById(id);

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return Ok(response.dataList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetSolicitudTrasladoToTransferencia(int id)
        {
            var response = await _repository.SolicitudTraslado.GetSolicitudTrasladoToTransferencia(id);

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return Ok(response.data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSolicitudTrasladoDetalleByIdAndLine([FromQuery] FilterRequestDto value)
        {
            var response = await _repository.SolicitudTraslado.GetSolicitudTrasladoDetalleByIdAndLine(value.ReturnValue());

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return Ok(response.data);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] SolicitudTrasladoCreateRequestDto value)
        {
            var filterRequestDto = new FilterRequestDto();

            if (value == null)
            {
                return BadRequest("No hay registros a crear ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            if(!value.IsNotRestAlmacen)
            {
                foreach (var linea in value.Linea)
                {
                    // Validación de almacén de origen
                    filterRequestDto = new FilterRequestDto() { Cod1 = linea.FromWhsCod, Id1 = value.CodSede };
                    var objectOrigen = await _repository.AlmacenSap.GetExisteByCodeAndSede(filterRequestDto.ReturnValue());

                    if (objectOrigen.ResultadoCodigo == -1)
                    {
                        return BadRequest(objectOrigen);
                    }

                    // Validación de almacén de destino
                    filterRequestDto = new FilterRequestDto() { Cod1 = linea.WhsCode, Id1 = value.CodSede };
                    var objectDestino = await _repository.AlmacenSap.GetExisteByCodeAndSede(filterRequestDto.ReturnValue());

                    if (objectDestino.ResultadoCodigo == -1)
                    {
                        return BadRequest(objectDestino);
                    }
                }
            }

            var objectNew = await _repository.SolicitudTraslado.SetCreate(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] SolicitudTrasladoUpdateRequestDto value)
        {
            var filterRequestDto = new FilterRequestDto();

            if (value == null)
            {
                return BadRequest("No hay registros a actualizar ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            if (!value.IsNotRestAlmacen)
            {
                foreach (var linea in value.Linea)
                {
                    // Validación de almacén de origen
                    filterRequestDto = new FilterRequestDto() { Cod1 = linea.FromWhsCod, Id1 = value.CodSede };
                    var objectOrigen = await _repository.AlmacenSap.GetExisteByCodeAndSede(filterRequestDto.ReturnValue());

                    if (objectOrigen.ResultadoCodigo == -1)
                    {
                        return BadRequest(objectOrigen);
                    }

                    // Validación de almacén de destino
                    filterRequestDto = new FilterRequestDto() { Cod1 = linea.WhsCode, Id1 = value.CodSede };
                    var objectDestino = await _repository.AlmacenSap.GetExisteByCodeAndSede(filterRequestDto.ReturnValue());

                    if (objectDestino.ResultadoCodigo == -1)
                    {
                        return BadRequest(objectDestino);
                    }
                }
            }

            var objectNew = await _repository.SolicitudTraslado.SetUpdate(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetClose([FromBody] SolicitudTrasladoCloseRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("No hay registros a cerrar ..!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo no válido ..!");
            }

            var objectNew = await _repository.SolicitudTraslado.SetClose(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }
    }
}
