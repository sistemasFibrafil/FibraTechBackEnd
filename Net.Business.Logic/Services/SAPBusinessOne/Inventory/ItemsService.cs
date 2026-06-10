using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.Items.Update;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Inventory;
using Net.Business.Logic.Mappers.SAPBusinessOne.Inventory.Items.Update;
namespace Net.Business.Logic.Services.SAPBusinessOne.Inventory
{
    public class ItemsService
        (
            IRepositoryWrapper repository,
            IValidator<List<ItemsUpdateMassiveRequestDto>> validatorUpdateMassive
        ) : IItemsService
    {
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<List<ItemsUpdateMassiveRequestDto>> _validatorUpdateMassive = validatorUpdateMassive;


        public async Task<ResultadoTransaccionResponse<object>> SetUpdateMassive(List<ItemsUpdateMassiveRequestDto> dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorUpdateMassive.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }

                // 🔹 REPOSITORY (SAP)
                var entity = ItemsUpdateMassiveMapper.ToEntity(dto);
                var result = await _repository.Items.SetUpdateMassive(entity);

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
