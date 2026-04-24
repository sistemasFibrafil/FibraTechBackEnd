using System;
namespace Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Filter
{
    public class TransferenciaStockFilterEntity
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocStatus { get; set; }
        public string? SearchText { get; set; }
    }
}
