using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class PurchaseRequestQueryEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; } = string.Empty;
        public string DocStatus { get; set; } = string.Empty;
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime ReqDate { get; set; }
        public int ReqType { get; set; }
        public string? Requester { get; set; }
        public string? ReqName { get; set; }
        public short? Branch { get; set; }
        public short? Department { get; set; }
        public string? Notify { get; set; }
        public string? Email { get; set; }
        public string? DocType { get; set; }
        public int? OwnerCode { get; set; }
        public string? Comments { get; set; }

        public List<PurchaseRequest1QueryEntity> Lines { get; set; } = new List<PurchaseRequest1QueryEntity>();
    }
}
