using System;
namespace Net.Business.Entities.SAPBusinessOne.Drafts.Filter
{
    public class DraftsDocumentReportFilterEntity
    {
        public string? User { get; set; }
        public bool Pending { get; set; }
        public string? DraftDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Orders { get; set; }
    }
}
