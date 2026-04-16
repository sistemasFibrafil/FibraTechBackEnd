using System;
using Net.Data;
using System.Linq;
using FluentValidation;
using Net.Business.DTO.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne;
using Microsoft.AspNetCore.Authorization;
using Net.Business.Services.Mappers.SAPBusinessOne;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Inventory.InventoryTransactions
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class InventoryTransferRequestController(IRepositoryWrapper repository, IValidator<InventoryTransferRequestCreateRequestDto> validatorCreate, IValidator<InventoryTransferRequestUpdateRequestDto> validatorUpdate) : ControllerBase
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<InventoryTransferRequestCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<InventoryTransferRequestUpdateRequestDto> _validatorUpdate = validatorUpdate;


        #region <<< CONSULTAS >>>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListByFilter([FromQuery] InventoryTransferRequestFilterDto value)
        {
            var result = await _repository.InventoryTransferRequest.GetListByFilter(value.ReturnValue());

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
            var result = await _repository.InventoryTransferRequest.GetByDocEntry(docEntry);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOpen()
        {
            var result = await _repository.InventoryTransferRequest.GetListOpen();

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
        public async Task<IActionResult> GetToTransferenciaByDocEntry(int docEntry)
        {
            var result = await _repository.InventoryTransferRequest.GetToTransferenciaByDocEntry(docEntry);

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.data);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListNotPicking()
        {
            var result = await _repository.InventoryTransferRequest.GetListNotPicking();

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return Ok(result.dataList);
        }

        #endregion


        #region <<< OPERACIONES >>>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCreate([FromBody] InventoryTransferRequestCreateRequestDto dto)
        {
            var validationResult = await _validatorCreate.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                }));
            }


            #region <<< VALIDACIÓN DE PERMISOS LOGÍSTICOS >>>

            var permisos = await _repository.LogisticUser.GetValidateByUser(new LogisticUserValidatedFindRequestDto { ObjectType = dto.ObjType ,IdUsuario = dto.U_UsrCreate }.ReturnValue());

            if(permisos.data == null)
            {
                return BadRequest(new { ResultadoCodigo = -1, ResultadoDescripcion = "No cuentas con permisos logísticos para realizar esta operación." });
            }

            if (!permisos.data.SuperUser)
            {
                for (int i = 0; i < dto.Lines.Count; i++)
                {
                    var permiso = permisos.data.Permissions.FirstOrDefault(p => p.WhsCode == dto.Lines[i].FromWhsCod && p.ToWhsCode == dto.Lines[i].WhsCode);
                    if (permiso == null)
                    {
                        return BadRequest(new { ResultadoCodigo = -1, ResultadoDescripcion = $"No tienes permiso para realizar operaciones de almacén <b>{dto.Lines[i].FromWhsCod}</b> a <b>{dto.Lines[i].WhsCode}</b>. Línea {i + 1}." });
                    }
                }
            }

            #endregion


            var entity = InventoryTransferRequestCreateMapper.ToEntity(dto);
            var result = await _repository.InventoryTransferRequest.SetCreate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUpdate([FromBody] InventoryTransferRequestUpdateRequestDto dto)
        {
            var validationResult = await _validatorUpdate.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                }));
            }


            #region <<< VALIDACIÓN DE PERMISOS LOGÍSTICOS >>>

            var permisos = await _repository.LogisticUser.GetValidateByUser(new LogisticUserValidatedFindRequestDto { ObjectType = dto.ObjType, IdUsuario = dto.U_UsrUpdate }.ReturnValue());

            if (permisos.data == null)
            {
                return BadRequest(new { ResultadoCodigo = -1, ResultadoDescripcion = "No cuentas con permisos logísticos para realizar esta operación." });
            }

            if (!permisos.data.SuperUser)
            {
                for (int i = 0; i < dto.Lines.Count; i++)
                {
                    var permiso = permisos.data.Permissions.FirstOrDefault(p => p.WhsCode == dto.Lines[i].FromWhsCod && p.ToWhsCode == dto.Lines[i].WhsCode);
                    if (permiso == null)
                    {
                        return BadRequest(new { ResultadoCodigo = -1, ResultadoDescripcion = $"No tienes permiso para realizar operaciones de almacén <b>{dto.Lines[i].FromWhsCod}</b> a <b>{dto.Lines[i].WhsCode}</b>. Línea {i + 1}." });
                    }
                }
            }

            #endregion


            var entity = InventoryTransferRequestUpdateMapper.ToEntity(dto);
            var result = await _repository.InventoryTransferRequest.SetUpdate(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetClose([FromBody] InventoryTransferRequestCloseRequestDto value)
        {
            var result = await _repository.InventoryTransferRequest.SetClose(value.ReturnValue());

            if (result.ResultadoCodigo == -1)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        #endregion


        #region <<< IMPRESIONES >>>

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetFormatoPdfByDocEntry(int id)
        {
            var objectGetById = await _repository.InventoryTransferRequest.GetFormatoPdfByDocEntry(id);

            var nombreArchivo = string.Format("Solicitud de traslado - {0}", DateTime.Now.ToString("dd-MM-yyyy").ToString());

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", nombreArchivo + ".pdf");

            return pdf;
        }

        #endregion
    }
}
