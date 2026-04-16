using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class InventoryTransferRequestCloseRequestDto
    {
        public int DocEntry { get; set; }
        public int U_UsrUpdate { get; set; }

        public InventoryTransferRequestCloseEntity ReturnValue()
        {
            return new InventoryTransferRequestCloseEntity()
            {
                DocEntry = DocEntry,
                U_UsrUpdate = U_UsrUpdate,
            };
        }
    }
}
