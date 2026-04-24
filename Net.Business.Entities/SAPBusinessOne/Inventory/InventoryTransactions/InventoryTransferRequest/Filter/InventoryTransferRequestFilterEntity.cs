using System;
namespace Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Filter
{
    public class InventoryTransferRequestFilterEntity
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocStatus { get; set; }
        public string? SearchText { get; set; }
    }
}
