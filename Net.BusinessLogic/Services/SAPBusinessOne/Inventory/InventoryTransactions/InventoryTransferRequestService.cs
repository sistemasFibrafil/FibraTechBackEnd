using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Net.Business.DTO.Web;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Update;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Update;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Inventory.InventoryTransactions;
namespace Net.BusinessLogic.Services.SAPBusinessOne.Inventory.InventoryTransactions
{
    public class InventoryTransferRequestService
        (
            IRepositoryWrapper repository,
            IValidator<InventoryTransferRequestCloseRequestDto> validatorClose,
            IValidator<InventoryTransferRequestCreateRequestDto> validatorCreate,
            IValidator<InventoryTransferRequestUpdateRequestDto> validatorUpdate
        ) : IInventoryTransferRequestService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<InventoryTransferRequestCloseRequestDto> _validatorClose = validatorClose;
        private readonly IValidator<InventoryTransferRequestCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<InventoryTransferRequestUpdateRequestDto> _validatorUpdate = validatorUpdate;


        #region 🔒 VALIDACIÓN DE PERMISOS

        private async Task<ResultadoTransaccionResponse<object>?> ValidarPermisos(
            string? objType,
            int userId,
            dynamic lines)
        {
            var permisos = await _repository.LogisticUser
                .GetValidateByUser(new LogisticUserValidatedFindRequestDto
                {
                    ObjectType = objType,
                    IdUsuario = userId
                }.ReturnValue());

            if (permisos.data == null)
            {
                return ResponseHelper.Error<object>(
                    "No cuentas con permisos logísticos para realizar esta operación."
                );
            }

            if (!permisos.data.SuperUser)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    var permiso = permisos.data.Permissions.FirstOrDefault(p =>
                        p.WhsCode == lines[i].FromWhsCod &&
                        p.ToWhsCode == lines[i].WhsCode);

                    if (permiso == null)
                    {
                        return ResponseHelper.Error<object>(
                            $"No tienes permiso para operar de {lines[i].FromWhsCod} a {lines[i].WhsCode}. Línea {i + 1}"
                        );
                    }
                }
            }

            return null;
        }

        #endregion


        public async Task<ResultadoTransaccionResponse<object>> SetCreate(InventoryTransferRequestCreateRequestDto dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorCreate.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }


                // 🔹 VALIDACIÓN PERMISOS
                var errorPermiso = await ValidarPermisos(dto.ObjType, dto.U_UsrCreate, dto.Lines);
                if (errorPermiso != null)
                    return errorPermiso;


                // 🔹 REPOSITORY (SAP)
                var entity = InventoryTransferRequestCreateMapper.ToEntity(dto);
                var result = await _repository.InventoryTransferRequest.SetCreate(entity);

                if (result.ResultadoCodigo == -1)
                    return ResponseHelper.From(result);

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

        public async Task<ResultadoTransaccionResponse<object>> SetUpdate(InventoryTransferRequestUpdateRequestDto dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorUpdate.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }


                // 🔹 VALIDACIÓN PERMISOS
                var errorPermiso = await ValidarPermisos(dto.ObjType, dto.U_UsrUpdate, dto.Lines);
                if (errorPermiso != null)
                    return errorPermiso;


                // 🔹 REPOSITORY (SAP)
                var entity = InventoryTransferRequestUpdateMapper.ToEntity(dto);
                var result = await _repository.InventoryTransferRequest.SetUpdate(entity);

                if (result.ResultadoCodigo == -1)
                    return ResponseHelper.From(result);

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

        public async Task<ResultadoTransaccionResponse<object>> SetClose(InventoryTransferRequestCloseRequestDto dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorClose.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }


                // 🔹 REPOSITORY (SAP)
                var entity = InventoryTransferRequestCloseMapper.ToEntity(dto);
                var result = await _repository.InventoryTransferRequest.SetClose(entity);

                if (result.ResultadoCodigo == -1)
                    return ResponseHelper.From(result);

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }
    }
}
