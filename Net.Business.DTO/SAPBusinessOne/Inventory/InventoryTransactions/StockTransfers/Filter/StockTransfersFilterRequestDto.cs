using System;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Filter;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class StockTransfersFilterRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocStatus { get; set; }
        public string? SearchText { get; set; }
        public TransferenciaStockFilterEntity ReturnValue()
        {
            return new TransferenciaStockFilterEntity()
            {
                SearchText = SearchText,
                DocStatus = DocStatus,
                StartDate = StartDate,
                EndDate = EndDate,
            };
        }
    }
}
