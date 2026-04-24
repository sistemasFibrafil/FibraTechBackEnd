using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Close;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Create;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Update;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.DeliveryNotes.Close;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.DeliveryNotes.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.DeliveryNotes.Update;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Sales;

namespace Net.BusinessLogic.Services.SAPBusinessOne.Sales
{
    public class DeliveryNotesService
        (
            IRepositoryWrapper repository,
            IValidator<DeliveryNotesCloseRequestDto> validatorClose,
            IValidator<DeliveryNotesCreateRequestDto> validatorCreate,
            IValidator<DeliveryNotesUpdateRequestDto> validatorUpdate,
            IValidator<DeliveryNotesCancelRequestDto> validatorCancel
        ) : IDeliveryNotesService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<DeliveryNotesCloseRequestDto> _validatorClose = validatorClose;
        private readonly IValidator<DeliveryNotesCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<DeliveryNotesUpdateRequestDto> _validatorUpdate = validatorUpdate;
        private readonly IValidator<DeliveryNotesCancelRequestDto> _validatorCancel = validatorCancel;

        public async Task<ResultadoTransaccionResponse<object>> SetCreate(DeliveryNotesCreateRequestDto dto)
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
                var entity = DeliveryNotesCreateMapper.ToEntity(dto);
                var result = await _repository.DeliveryNotes.SetCreate(entity);

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

        public async Task<ResultadoTransaccionResponse<object>> SetUpdate(DeliveryNotesUpdateRequestDto dto)
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
                var entity = DeliveryNotesUpdateMapper.ToEntity(dto);
                var result = await _repository.DeliveryNotes.SetUpdate(entity);

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

        public async Task<ResultadoTransaccionResponse<object>> SetCancel(DeliveryNotesCancelRequestDto dto)
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
                var entity = DeliveryNotesCancelMapper.ToEntity(dto);
                var result = await _repository.DeliveryNotes.SetCancel(entity);

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

        public async Task<ResultadoTransaccionResponse<object>> SetClose(DeliveryNotesCloseRequestDto dto)
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
                var entity = DeliveryNotesCloseMapper.ToEntity(dto);
                var result = await _repository.DeliveryNotes.SetClose(entity);

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
