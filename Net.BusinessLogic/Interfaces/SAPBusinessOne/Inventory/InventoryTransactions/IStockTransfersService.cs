using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Update;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.Inventory.InventoryTransactions
{
    public interface IStockTransfersService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(StockTransfersCreateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetUpdate(StockTransfersUpdateRequestDto dto);
    }
}
