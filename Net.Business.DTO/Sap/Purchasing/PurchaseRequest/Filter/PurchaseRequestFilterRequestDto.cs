using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class PurchaseRequestFilterRequestDto
    {
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public string DocStatus { get; set; } = null;
        public string SearchText { get; set; } = null;

        public PurchaseRequestFilterEntity ReturnValue()
        {
            return new PurchaseRequestFilterEntity()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                DocStatus = DocStatus,
                SearchText = SearchText
            };
        }
    }
}
