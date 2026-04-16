using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class InvoicesFilterRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocStatus { get; set; }
        public string? DocSubType { get; set; }
        public string? isIns { get; set; }
        public string? SearchText { get; set; }

        public InvoicesFilterEntity ReturnValue()
        {
            return new InvoicesFilterEntity
            {
                StartDate = StartDate,
                EndDate = EndDate,
                DocStatus = DocStatus,
                DocSubType = DocSubType,
                isIns = isIns,
                SearchText = SearchText
            };
        }
    }
}
