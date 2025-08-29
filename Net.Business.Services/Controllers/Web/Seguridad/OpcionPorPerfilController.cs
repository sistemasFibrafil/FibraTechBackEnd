using Net.Data;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Web.Seguridad
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OpcionPorPerfilController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public OpcionPorPerfilController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtener lista de registros
        /// </summary>
        /// <param name="value">Este es el cuerpo para enviar los parametros</param>
        /// <returns>Lista del maestro de Calidad registrado</returns>
        /// <response code="200">Devuelve el listado completo </response>
        /// <response code="404">Si no existen datos</response>   
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllSeleccionado([FromQuery] OpcionPorPerfilFindRequestDto value)
        {

            var objectGetAll = await _repository.OpcionxPerfil.GetAllSeleccionado(value.RetornarOpcionxPerfil());

            if (objectGetAll == null)
            {
                return NotFound();
            }

            return Ok(objectGetAll);
        }

        /// <summary>
        /// Obtener lista de registros
        /// </summary>
        /// <param name="value">Este es el cuerpo para enviar los parametros</param>
        /// <returns>Lista del maestro de Calidad registrado</returns>
        /// <response code="200">Devuelve el listado completo </response>
        /// <response code="404">Si no existen datos</response>   
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllPorSeleccionar([FromQuery] OpcionPorPerfilFindRequestDto value)
        {

            var objectGetAll = await _repository.OpcionxPerfil.GetAllPorSeleccionar(value.RetornarOpcionxPerfil());

            if (objectGetAll == null)
            {
                return NotFound();
            }

            return Ok(objectGetAll);
        }

        /// <summary>
        /// Crear una nueva registro
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Id del registro creado</returns>
        /// <response code="201">Devuelve el elemento recién creado</response>
        /// <response code="400">Si el objeto enviado es nulo o invalido</response>  
        /// <response code="500">Algo salio mal guardando el registro</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] List<OpcionPorPerfilInsertarRequestDto> value)
        {
            if (value == null)
            {
                return BadRequest("Master object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }
            int ObjectNew = 0;

            foreach (var item in value)
            {
                ObjectNew = await _repository.OpcionxPerfil.Create(item.RetornarOpcionxPerfil());
            }

            return Ok();
        }

        /// <summary>
        /// Eliminar un registro existente
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<response code="204">Eliminado Satisfactoriamente</response>
        ///<response code="400">Si el objeto enviado es nulo o invalido</response>
        ///<response code="409">Si ocurrio un conflicto</response>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete([FromBody] List<OpcionPorPerfilEliminarRequestDto> value)
        {
            if (value == null)
            {
                return BadRequest(ModelState);
            }
            foreach (var item in value)
            {
                await _repository.OpcionxPerfil.Delete(item.RetornarOpcionxPerfil());
            }

            return NoContent();
        }
    }
}
