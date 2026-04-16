using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class PurchaseRequestFilterRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocStatus { get; set; }
        public string? SearchText { get; set; }

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
