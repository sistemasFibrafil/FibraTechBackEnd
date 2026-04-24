using Net.Data;
using FluentValidation;
using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Purchasing;
namespace Net.BusinessLogic.Services.SAPBusinessOne.Purchasing
{
    public class PurchaseRequestService
        (
            IRepositoryWrapper repository,
            IValidator<PurchaseRequestCloseRequestDto> validatorClose,
            IValidator<PurchaseRequestCreateRequestDto> validatorCreate,
            IValidator<PurchaseRequestUpdateRequestDto> validatorUpdate
        ) : IPurchaseRequestService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<PurchaseRequestCloseRequestDto> _validatorClose = validatorClose;
        private readonly IValidator<PurchaseRequestCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<PurchaseRequestUpdateRequestDto> _validatorUpdate = validatorUpdate;


        public async Task<ResultadoTransaccionResponse<object>> SetCreate(PurchaseRequestCreateRequestDto dto)
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


                // 🔹 REPOSITORY (SAP)
                var entity = PurchaseRequestCreateMapper.ToEntity(dto);
                var result = await _repository.PurchaseRequest.SetCreate(entity);

                if (result.ResultadoCodigo == -1)
                {
                    return ResponseHelper.From(result);
                }

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

        public async Task<ResultadoTransaccionResponse<object>> SetUpdate(PurchaseRequestUpdateRequestDto dto)
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


                // 🔹 REPOSITORY (SAP)
                var entity = PurchaseRequestUpdateMapper.ToEntity(dto);
                var result = await _repository.PurchaseRequest.SetUpdate(entity);

                if (result.ResultadoCodigo == -1)
                {
                    return ResponseHelper.From(result);
                }

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

        public async Task<ResultadoTransaccionResponse<object>> SetClose(PurchaseRequestCloseRequestDto dto)
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
                var entity = PurchaseRequestCloseMapper.ToEntity(dto);
                var result = await _repository.PurchaseRequest.SetClose(entity);

                if (result.ResultadoCodigo == -1)
                {
                    return ResponseHelper.From(result);
                }

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }
    }
}
