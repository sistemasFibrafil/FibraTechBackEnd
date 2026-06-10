using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate;
namespace Net.Business.Logic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate
{
    public class InventoryTransferRequestLinesValidateMapper
    {
        public static List<InventoryTransferRequestLinesValidateEntity> ToEntity(List<InventoryTransferRequestLinesValidateRequestDto> dto)
        {
            return [.. dto.Select(l => new InventoryTransferRequestLinesValidateEntity
            {
                ItemCode = l.ItemCode,
                FromWhsCod = l.FromWhsCod,
                WhsCode = l.WhsCode,
                U_tipoOpT12 = l.U_tipoOpT12,
                UnitMsr = l.UnitMsr,
                Quantity = l.Quantity
            })];
        }
    }
}
