using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Purchasing;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
using Net.Business.Logic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.Logic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.Logic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.Business.Logic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Business.Logic.Services.SAPBusinessOne.Purchasing
{
    public class PurchaseRequestService
        (
            IRepositoryWrapper repository,
            IValidator<PurchaseRequestCloseRequestDto> validatorClose,
            IValidator<PurchaseRequestCreateRequestDto> validatorCreate,
            IValidator<PurchaseRequestUpdateRequestDto> validatorUpdate,
            IValidator<List<PurchaseRequestLinesItemsValidateRequestDto>> validatorItemsValidateLines,
            IValidator<List<PurchaseRequestLinesServicesValidateRequestDto>> validatorServicesValidateLines
        ) : IPurchaseRequestService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<PurchaseRequestCloseRequestDto> _validatorClose = validatorClose;
        private readonly IValidator<PurchaseRequestCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<PurchaseRequestUpdateRequestDto> _validatorUpdate = validatorUpdate;
        private readonly IValidator<List<PurchaseRequestLinesItemsValidateRequestDto>> _validatorItemsValidateLines = validatorItemsValidateLines;
        private readonly IValidator<List<PurchaseRequestLinesServicesValidateRequestDto>> _validatorServicesValidateLines = validatorServicesValidateLines;


        public async Task<ResultadoTransaccionResponse<object>> SetValidateLinesItemsExcel(List<PurchaseRequestLinesItemsValidateRequestDto> dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorItemsValidateLines.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }


                // 🔹 REPOSITORY (SAP)
                var entity = PurchaseRequestLinesItemsValidateMapper.ToEntity(dto);
                var result = await _repository.PurchaseRequest.SetValidateLinesItemsExcel(entity);

                if (result.ResultadoCodigo == -1)
                {
                    return ResponseHelper.From(result);
                }

                return new ResultadoTransaccionResponse<object>
                {
                    IdRegistro = result.IdRegistro,
                    ResultadoCodigo = result.ResultadoCodigo,
                    ResultadoDescripcion = result.ResultadoDescripcion,
                    DataList = result.DataList?.Cast<object>().ToList() ?? []
                };
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

        public async Task<ResultadoTransaccionResponse<object>> SetValidateLinesServicesExcel(List<PurchaseRequestLinesServicesValidateRequestDto> dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorServicesValidateLines.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }


                // 🔹 REPOSITORY (SAP)
                var entity = PurchaseRequestLinesServicesValidateMapper.ToEntity(dto);
                var result = await _repository.PurchaseRequest.SetValidateLinesServicsExcel(entity);

                if (result.ResultadoCodigo == -1)
                {
                    return ResponseHelper.From(result);
                }

                return new ResultadoTransaccionResponse<object>
                {
                    IdRegistro = result.IdRegistro,
                    ResultadoCodigo = result.ResultadoCodigo,
                    ResultadoDescripcion = result.ResultadoDescripcion,
                    DataList = result.DataList?.Cast<object>().ToList() ?? []
                };
            }
            catch (Exception ex)
            {
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

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
