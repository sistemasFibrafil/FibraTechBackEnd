using Net.Data;
using FluentValidation;
using Net.CrossCotting;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Administration.SystemInitialization;
using Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
namespace Net.BusinessLogic.Services.SAPBusinessOne.Administration.SystemInitialization
{
    internal class DocumentSeriesConfigurationService
        (
            IRepositoryWrapper repository,
            IValidator<DocumentSeriesConfigurationCreateRequestDto> validatorCreate
        ) : IDocumentSeriesConfigurationService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<DocumentSeriesConfigurationCreateRequestDto> _validatorCreate = validatorCreate;


        public async Task<ResultadoTransaccionResponse<object>> SetCreate(DocumentSeriesConfigurationCreateRequestDto dto)
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
                var entity = DocumentSeriesConfigurationCreateMapper.ToEntity(dto);
                var result = await _repository.DocumentSeriesConfiguration.SetCreate(entity);

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
