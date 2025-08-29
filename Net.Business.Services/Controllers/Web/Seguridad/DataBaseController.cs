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
    public class DataBaseController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public DataBaseController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// Obtener lista de registros
        /// </summary>
        /// <returns>Lista del maestro de Calidad registrado</returns>
        /// <response code="200">Devuelve el listado completo </response>
        /// <response code="404">Si no existen datos</response>   
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            DataBaseFinRequestDto value = new DataBaseFinRequestDto();
            var objectGetAll = await _repository.DataBase.GetAll(value.RetornaDataBase());

            if (objectGetAll == null)
            {
                return NotFound();
            }

            return Ok(objectGetAll);
        }
    }
}
