using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Update;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.Inventory.InventoryTransactions
{
    public interface IInventoryTransferRequestService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(InventoryTransferRequestCreateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetUpdate(InventoryTransferRequestUpdateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetClose(InventoryTransferRequestCloseRequestDto dto);
    }
}
