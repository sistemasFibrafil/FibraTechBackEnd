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
    public class PersonaController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public PersonaController(IRepositoryWrapper repository)
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
        public async Task<IActionResult> GetAll([FromQuery] PersonaFindRequestDto value)
        {

            var objectGetAll = await _repository.Persona.GetAll(value.RetornaPersona());

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
        [HttpGet("{id}", Name = "GetbyIdPersona")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetbyIdPersona(int id)
        {
            var objectGetById = await _repository.Persona.GetById(new PersonaFindRequestDto { IdPersona = id }.RetornaPersona());

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
        public async Task<IActionResult> Create([FromBody] PersonaInsertarRequestDto value)
        {
            if (value == null)
            {
                return BadRequest("Master object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var ObjectNew = await _repository.Persona.Create(value.RetornaPersona());

            if (ObjectNew.ResultadoCodigo == -1)
            {
                return BadRequest(ObjectNew);
            }

            return CreatedAtRoute("GetbyIdPersona", new { id = ObjectNew.IdRegistro }, ObjectNew.IdRegistro);
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
        public async Task<IActionResult> Update([FromBody] PersonaUpdateRequestDto value)
        {
            if (value == null)
            {
                return BadRequest(ModelState);
            }

            var response = await _repository.Persona.Update(value.RetornaPersona());

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

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
        public async Task<IActionResult> Delete([FromBody] PersonaDeleteRequestDto value)
        {
            if (value == null)
            {
                return BadRequest(ModelState);
            }

            var response = await _repository.Persona.Delete(value.RetornaPersona());

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }

            return NoContent();
        }
    }
}
