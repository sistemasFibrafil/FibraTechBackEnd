using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class PurchaseRequestFilterEntity
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocStatus { get; set; }
        public string? SearchText { get; set; }
    }
}
