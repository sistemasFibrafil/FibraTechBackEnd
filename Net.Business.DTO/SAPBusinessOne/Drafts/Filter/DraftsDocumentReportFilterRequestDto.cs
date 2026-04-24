using System;
namespace Net.Business.DTO.SAPBusinessOne.Drafts.Filter
{
    public class DraftsDocumentReportFilterRequestDto
    {
        public string? User { get; set; }
        public bool Pending { get; set; }
        public string? DraftDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Orders { get; set; }
    }
}
