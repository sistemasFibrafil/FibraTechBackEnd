using Net.Data;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Net.Business.DTO;
namespace Net.Business.Services.Controllers.Web.Inventario.OperacionesStock
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TransferenciaStockController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public TransferenciaStockController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListFiltro([FromQuery] FilterRequestDto value)
        {
            var objectGetAll = await _repository.TransferenciaStock.GetListByFiltro(value.ReturnValue());

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
            var response = await _repository.TransferenciaStock.GetById(id);

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
        public async Task<IActionResult> SetCreate([FromBody] TransferenciaStockCreateRequestDto value)
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

            var objectNew = await _repository.TransferenciaStock.SetCreate(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] TransferenciaStockUpdateRequestDto value)
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

            var objectNew = await _repository.TransferenciaStock.SetUpdate(value.ReturnValue());

            if (objectNew.ResultadoCodigo == -1)
            {
                return BadRequest(objectNew);
            }

            return NoContent();
        }
    }
}
