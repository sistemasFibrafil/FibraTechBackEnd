using System;
namespace Net.Business.Entities.Sap
{
    public class TransferenciaStockFilterEntity
    {
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public string DocStatus { get; set; } = null;
        public string SearchText { get; set; } = null;
    }
}
