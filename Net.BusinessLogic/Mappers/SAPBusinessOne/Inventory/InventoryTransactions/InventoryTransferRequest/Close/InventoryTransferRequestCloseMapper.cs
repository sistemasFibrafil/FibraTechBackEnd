using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close
{
    public class InventoryTransferRequestCloseMapper
    {
        public static InventoryTransferRequestCloseEntity ToEntity(InventoryTransferRequestCloseRequestDto dto)
        {
            return new InventoryTransferRequestCloseEntity()
            {
                DocEntry = dto.DocEntry,
                U_UsrUpdate = dto.U_UsrUpdate,
            };
        }
    }
}
