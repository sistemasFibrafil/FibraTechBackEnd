using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class InventoryTransferRequestFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocStatus { get; set; }
        public string? SearchText { get; set; }
        public InventoryTransferRequestFilterEntity ReturnValue()
        {
            return new InventoryTransferRequestFilterEntity()
            {
                SearchText = SearchText,
                DocStatus = DocStatus,
                StartDate = StartDate,
                EndDate = EndDate,
            };
        }
    }
}
