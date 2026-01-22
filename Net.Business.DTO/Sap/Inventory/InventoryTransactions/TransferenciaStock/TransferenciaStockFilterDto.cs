using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TransferenciaStockFilterDto
    {
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public string DocStatus { get; set; } = null;
        public string SearchText { get; set; } = null;
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
