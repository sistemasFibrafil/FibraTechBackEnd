using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.BusinessPartners;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Drivers.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.BusinessPartners.Drivers.Create;
namespace Net.BusinessLogic.Services.SAPBusinessOne.BusinessPartners
{
    public class DriversService
        (
            IRepositoryWrapper repository,
            IValidator<DriversCreateRequestDto> validatorCreate
        ) : IDriversService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<DriversCreateRequestDto> _validatorCreate = validatorCreate;


        public async Task<ResultadoTransaccionResponse<object>> SetCreate(DriversCreateRequestDto dto)
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
                var entity = DriversCreateMapper.ToEntity(dto);
                var result = await _repository.Drivers.SetCreate(entity);

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
