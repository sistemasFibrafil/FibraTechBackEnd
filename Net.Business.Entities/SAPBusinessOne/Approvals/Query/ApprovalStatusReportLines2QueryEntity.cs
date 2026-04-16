using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class ApprovalStatusReportLines2QueryEntity
    {
        public int WddCode { get; set; }
        public string? StapaName { get; set; }
        public string? AuthorizerName { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateTime { get; set; }
        public string? UpdateTimeString { get; set; }
        public string? Remarks { get; set; }
    }
}
