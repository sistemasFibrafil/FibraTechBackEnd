using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Inventory.Items.Update;
namespace Net.Business.Logic.Interfaces.SAPBusinessOne.Inventory
{
    public interface IItemsService
    {
        Task<ResultadoTransaccionResponse<object>> SetUpdateMassive(List<ItemsUpdateMassiveRequestDto> dto);
    }
}
