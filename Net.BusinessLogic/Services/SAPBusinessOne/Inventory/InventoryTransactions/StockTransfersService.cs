using Net.Data;
using FluentValidation;
using Net.CrossCotting;
using Net.Business.DTO.Web;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Update;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Update;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Inventory.InventoryTransactions;
namespace Net.BusinessLogic.Services.SAPBusinessOne.Inventory.InventoryTransactions
{
    public class StockTransfersService
        (
            IRepositoryWrapper repository,
            IValidator<StockTransfersCreateRequestDto> validatorCreate,
            IValidator<StockTransfersUpdateRequestDto> validatorUpdate
        ) : IStockTransfersService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<StockTransfersCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<StockTransfersUpdateRequestDto> _validatorUpdate = validatorUpdate;


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


        public async Task<ResultadoTransaccionResponse<object>> SetCreate(StockTransfersCreateRequestDto dto)
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
                var entity = StockTransfersCreateMapper.ToEntity(dto);
                var result = await _repository.StockTransfers.SetCreate(entity);

                if (result.ResultadoCodigo == -1)
                    return ResponseHelper.From(result);

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

        public async Task<ResultadoTransaccionResponse<object>> SetUpdate(StockTransfersUpdateRequestDto dto)
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
                var entity = StockTransfersUpdateMapper.ToEntity(dto);
                var result = await _repository.StockTransfers.SetUpdate(entity);

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
