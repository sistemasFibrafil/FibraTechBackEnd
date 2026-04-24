using Net.Data;
using FluentValidation;
using Net.CrossCotting;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.BusinessPartners;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.BusinessPartners.Vehicles.Create;
namespace Net.BusinessLogic.Services.SAPBusinessOne.BusinessPartners
{
    public class VehiclesService
        (
            IRepositoryWrapper repository,
            IValidator<VehiclesCreateRequestDto> validatorCreate
        ) : IVehiclesService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<VehiclesCreateRequestDto> _validatorCreate = validatorCreate;


        public async Task<ResultadoTransaccionResponse<object>> SetCreate(VehiclesCreateRequestDto dto)
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
                var entity = VehiclesCreateMapper.ToEntity(dto);
                var result = await _repository.Vehicles.SetCreate(entity);

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
