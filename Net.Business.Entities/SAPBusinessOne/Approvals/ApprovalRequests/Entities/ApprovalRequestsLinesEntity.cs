using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Approvals.ApprovalRequests.Entities
{
    public class ApprovalRequestsLinesEntity
    {
        public int WddCode { get; set; }
        public int StepCode { get; set; }
        public int UserID { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdateDate { get; set; }
        public short? UpdateTime { get; set; }
        public string? Remarks { get; set; }
    }
}
