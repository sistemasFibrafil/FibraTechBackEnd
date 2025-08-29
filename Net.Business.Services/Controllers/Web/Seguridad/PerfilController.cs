using Net.Data;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Web.Seguridad
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PerfilController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public PerfilController(IRepositoryWrapper repository)
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
        public async Task<IActionResult> GetAll([FromQuery] PerfilFindRequestDto value)
        {

            var objectGetAll = await _repository.Perfil.GetAll(value.RetornarPeril());

            if (objectGetAll == null)
            {
                return NotFound();
            }

            return Ok(objectGetAll);
        }


        /// <summary>
        /// Obtener un registro individual segun el ID
        /// </summary>
        /// <param name="id">Id de Usuarii</param>
        /// <returns>Devuelve un solo registro</returns>
        /// <response code="200">Devuelve el listado completo </response>
        /// <response code="404">Si no existen datos</response>  
        [HttpGet("{id}", Name = "GetbyIdPerfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetbyIdPerfil(int id)
        {
            var objectGetById = await _repository.Perfil.GetById(new PerfilFindRequestDto { IdPerfil = id }.RetornarPeril());

            if (objectGetById == null)
            {
                return NotFound();
            }

            return Ok(objectGetById);
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] PerfilInsertarRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("Master object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            int ObjectNew = await _repository.Perfil.Create(value.RetornarPeril());

            if (ObjectNew == 0)
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {value.DescripcionPerfil}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetbyIdPerfil", new { id = ObjectNew }, ObjectNew);
        }

        /// <summary>
        /// Actualizar un registro existente
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <response code="204">Actualizado Satisfactoriamente</response>
        /// <response code="404">Si el objeto enviado es nulo o invalido</response>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] PerfilUpdateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest(ModelState);
            }

            await _repository.Perfil.Update(value.RetornarPeril());

            return NoContent();
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
        public async Task<IActionResult> Delete([FromBody] PerfilDeleteRequestDto value)
        {
            if (value == null)
            {
                return BadRequest(ModelState);
            }

            await _repository.Perfil.Delete(value.RetornarPeril());

            return NoContent();
        }
    }
}
