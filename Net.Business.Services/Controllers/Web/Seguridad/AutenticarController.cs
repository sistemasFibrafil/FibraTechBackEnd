using Net.Data;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
namespace Net.Business.Services.Controllers.Web.Seguridad
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    public class AutenticarController : ControllerBase
    {
        private readonly IRepositoryWrapper repository;

        public AutenticarController(IRepositoryWrapper repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public IActionResult Autenticar([FromBody] UsuarioAutenticarRequestDto request)
        {
            var response = repository.Usuario.Autenticar(request.UsuarioAutenticar());
            if (response.Result.ResultadoCodigo < 0)
            {
                return BadRequest(response.Result);
            }

            return Ok(response.Result.data);
        }

        [HttpPost]
        public IActionResult ObtienePermisosPorUsuario([FromBody] UsuarioDatosRequestDto request)
        {
            var response = repository.Usuario.ObtienePermisosPorUsuario(request.UsuarioDatos());

            if (response == null)
            {
                return BadRequest("Credenciales incorrectas");
            }

            return Ok(response.Result.data);
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
        public async Task<IActionResult> RecuperarPassword([FromBody] UsuarioRecuperarClaveDto value)
        {
            if (value == null)
            {
                return BadRequest(ModelState);
            }

            await repository.Usuario.RecuperarPassword(value.RetornaUsuario());

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidarToken([FromBody] DtoUsuarioTokenRequest value)
        {

            var data = await repository.Usuario.ValidarToken(value.UsuarioDatos());

            return Ok(data);
        }
    }
}