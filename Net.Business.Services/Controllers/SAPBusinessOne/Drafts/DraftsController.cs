using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
using Net.Business.Services.Mappers.SAPBusinessOne;
using Net.Business.Services.Mappers.SAPBusinessOne.Draft.Resend;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Drafts
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class DraftsController(IRepositoryWrapper repository) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;


        #region <<< CONSULTAS >>>

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

            return Ok(result.data);
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

            return Ok(result.data);
        }

        #endregion


        #region <<< OPERACIONES >>>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] DraftsCreateRequestDto dto)
        {
            var entity = DraftsCreateMapper.ToEntity(dto);
            var result = await _repository.Drafts.SetCreate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetResend([FromBody] DraftsResendRequestDto dto)
        {
            var entity = DraftsResendMapper.ToEntity(dto);
            var result = await _repository.Drafts.SetResend(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        #endregion
    }
}
