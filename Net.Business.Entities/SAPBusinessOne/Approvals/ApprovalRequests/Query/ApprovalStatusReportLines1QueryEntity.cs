using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Approvals.ApprovalRequests.Query
{
    public class ApprovalStatusReportLines1QueryEntity
    {
        public int WddCode { get; set; }
        public int DocEntry { get; set; }
        public int? DocNum { get; set; }
        public string? ObjType { get; set; }
        public DateTime CreateDate { get; set; }
        public short CreateTime { get; set; }
        public string? CreateTimeString { get; set; }
        public string? AuthorName { get; set; }
        public string? ModelName { get; set; }
        public string? ApproverStatus { get; set; }
        public string? IsDraft { get; set; }
        public string? Remarks { get; set; }
        public List<ApprovalStatusReportLines2QueryEntity> Lines { get; set; } = new List<ApprovalStatusReportLines2QueryEntity>();
    }
}
