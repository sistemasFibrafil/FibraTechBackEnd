using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Create;
using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Update;
using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Cancel;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.Invoices.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.Invoices.Update;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.Invoices.Cancel;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Sales;
namespace Net.BusinessLogic.Services.SAPBusinessOne.Sales
{
    public class InvoicesService
        (
            IRepositoryWrapper repository,
            IValidator<InvoicesCreateRequestDto> validatorCreate,
            IValidator<InvoicesUpdateRequestDto> validatorUpdate,
            IValidator<InvoicesCancelRequestDto> validatorCancel
        ) : IInvoicesService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<InvoicesCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<InvoicesUpdateRequestDto> _validatorUpdate = validatorUpdate;
        private readonly IValidator<InvoicesCancelRequestDto> _validatorCancel = validatorCancel;

        public async Task<ResultadoTransaccionResponse<object>> SetCreate(InvoicesCreateRequestDto dto)
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
                var entity = InvoicesCreateMapper.ToEntity(dto);
                var result = await _repository.Invoices.SetCreate(entity);

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

        public async Task<ResultadoTransaccionResponse<object>> SetUpdate(InvoicesUpdateRequestDto dto)
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
                var entity = InvoicesUpdateMapper.ToEntity(dto);
                var result = await _repository.Invoices.SetUpdate(entity);

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

        public async Task<ResultadoTransaccionResponse<object>> SetCancel(InvoicesCancelRequestDto dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorCancel.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }


                // 🔹 REPOSITORY (SAP)
                var entity = InvoicesCancelMapper.ToEntity(dto);
                var result = await _repository.Invoices.SetCancel(entity);

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
