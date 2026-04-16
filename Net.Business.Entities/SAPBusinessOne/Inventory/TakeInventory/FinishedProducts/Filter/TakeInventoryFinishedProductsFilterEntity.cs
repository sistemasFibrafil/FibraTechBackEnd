using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class TakeInventoryFinishedProductsFilterEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Usuario { get; set; }
        public string? WhsCode { get; set; }
        public string? Item { get; set; }
    }
}
