using System;
using Net.Data;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.FacturacionElectronica
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class GuiaElectronicaSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public GuiaElectronicaSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetEnviar([FromBody] FilterRequestDto value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.GuiaElectronicaSap.SetEnviar(value.ReturnValue());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }

                return Ok(value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
