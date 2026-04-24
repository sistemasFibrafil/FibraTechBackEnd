using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Approvals.ApprovalRequests.Query
{
    public class ApprovalStatusReportQueryEntity
    {
        public int Line { get; set; }
        public int Order { get; set; }
        public int DocEntry { get; set; }
        public int? DocNum { get; set; }
        public string? ObjType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateTime { get; set; }
        public string? CreateTimeString { get; set; }
        public string? IsDraft { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? AuthorName { get; set; }
        public string? DocumentStatus { get; set; }
        public string? Remarks { get; set; }
        public List<ApprovalStatusReportLines1QueryEntity> Lines { get; set; } = new List<ApprovalStatusReportLines1QueryEntity>();
    }
}
