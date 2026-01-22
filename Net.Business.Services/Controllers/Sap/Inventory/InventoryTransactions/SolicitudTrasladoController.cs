using System;
using Net.Data;
using System.Linq;
using Net.Business.DTO.Sap;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace Net.Business.Services.Controllers.Sap.Inventory.InventoryTransactions
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SolicitudTrasladoController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public SolicitudTrasladoController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetListOpen()
        {
            var result = await _repository.SolicitudTraslado.GetListOpen();

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] SolicitudTrasladoFilterDto value)
        {
            var result = await _repository.SolicitudTraslado.GetListByFilter(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetByDocEntry(int docEntry)
        {
            var result = await _repository.SolicitudTraslado.GetByDocEntry(docEntry);

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
        public async Task<IActionResult> GetToTransferenciaByDocEntry(int docEntry)
        {
            var result = await _repository.SolicitudTraslado.GetToTransferenciaByDocEntry(docEntry);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetListNotPicking()
        {
            var result = await _repository.SolicitudTraslado.GetListNotPicking();

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] SolicitudTrasladoCreateRequestDto value)
        {            
            var permisos = await _repository.LogisticUser.GetValidateByUser(new LogisticUserValidatedFindRequestDto { ObjectType = value.ObjType ,IdUsuario = value.U_UsrCreate.Value }.ReturnValue());

            if(permisos.data == null)
            {
                return BadRequest(new { ResultadoCodigo = -1, ResultadoDescripcion = "No cuentas con permisos logísticos para realizar esta operación." });
            }

            if (!permisos.data.SuperUser)
            {
                for (int i = 0; i < value.Lines.Count; i++)
                {
                    var permiso = permisos.data.Permissions.FirstOrDefault(p => p.WhsCode == value.Lines[i].FromWhsCod && p.ToWhsCode == value.Lines[i].WhsCode);
                    if (permiso == null)
                    {
                        return BadRequest(new { ResultadoCodigo = -1, ResultadoDescripcion = $"No tienes permiso para realizar operaciones de almacén <b>{value.Lines[i].FromWhsCod}</b> a <b>{value.Lines[i].WhsCode}</b>. Línea {i + 1}." });
                    }
                }
            }

            var result = await _repository.SolicitudTraslado.SetCreate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] SolicitudTrasladoUpdateRequestDto value)
        {
            var permisos = await _repository.LogisticUser.GetValidateByUser(new LogisticUserValidatedFindRequestDto { ObjectType = value.ObjType, IdUsuario = value.U_UsrUpdate.Value }.ReturnValue());

            if (permisos.data == null)
            {
                return BadRequest(new { ResultadoCodigo = -1, ResultadoDescripcion = "No cuentas con permisos logísticos para realizar esta operación." });
            }

            if (!permisos.data.SuperUser)
            {
                for (int i = 0; i < value.Lines.Count; i++)
                {
                    var permiso = permisos.data.Permissions.FirstOrDefault(p => p.WhsCode == value.Lines[i].FromWhsCod && p.ToWhsCode == value.Lines[i].WhsCode);
                    if (permiso == null)
                    {
                        return BadRequest(new { ResultadoCodigo = -1, ResultadoDescripcion = $"No tienes permiso para realizar operaciones de almacén <b>{value.Lines[i].FromWhsCod}</b> a <b>{value.Lines[i].WhsCode}</b>. Línea {i + 1}." });
                    }
                }
            }

            var result = await _repository.SolicitudTraslado.SetUpdate(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetClose([FromBody] SolicitudTrasladoCloseRequestDto value)
        {
            var result = await _repository.SolicitudTraslado.SetClose(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetFormatoPdfByDocEntry(int id)
        {
            var objectGetById = await _repository.SolicitudTraslado.GetFormatoPdfByDocEntry(id);

            var nombreArchivo = string.Format("Solicitud de traslado - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }
    }
}
