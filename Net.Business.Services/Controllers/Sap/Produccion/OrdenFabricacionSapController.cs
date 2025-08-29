using Net.Data;
using System.IO;
using Net.Business.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Produccion
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrdenFabricacionSapController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public OrdenFabricacionSapController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenFabricacionBySede([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.OrdenFabricacionSap.GetListOrdenFabricacionBySede(value.ReturnValue());

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetOrdenFabricacionExcelBySede([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectFile = await _repository.OrdenFabricacionSap.GetOrdenFabricacionExcelBySede(value.ReturnValue());

                objectFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectFile.data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOrdenFabricacionGeneralBySede([FromQuery] FilterRequestDto value)
        {
            var objectGetList = await _repository.OrdenFabricacionSap.GetListOrdenFabricacionGeneralBySede(value.ReturnValue());

            if (objectGetList == null)
            {
                return NotFound();
            }

            return Ok(objectGetList.dataList);
        }

        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetOrdenFabricacionGeneralExcelBySede([FromQuery] FilterRequestDto value)
        {
            try
            {
                var objectFile = await _repository.OrdenFabricacionSap.GetOrdenFabricacionGeneralExcelBySede(value.ReturnValue());

                objectFile.data.Seek(0, SeekOrigin.Begin);
                var file = objectFile.data.ToArray();

                return new FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
