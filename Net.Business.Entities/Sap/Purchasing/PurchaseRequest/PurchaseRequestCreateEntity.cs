using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class PurchaseRequestCreateEntity
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime ReqDate { get; set; }
        public string DocType { get; set; }
        public int ReqType { get; set; }
        public string Requester { get; set; }
        public string ReqName { get; set; }
        public Int16 Branch { get; set; }
        public Int16 Department { get; set; }
        public string Notify { get; set; }
        public string Email { get; set; }
        public int OwnerCode { get; set; }
        public string Comments { get; set; }
        public int? U_UsrCreate { get; set; }
        public List<PurchaseRequest1CreateEntity> Lines { get; set; } = new List<PurchaseRequest1CreateEntity>();
    }

    public class PurchaseRequest1CreateEntity
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string LineVendor { get; set; }
        public DateTime PqtReqDate { get; set; }
        public string AcctCode { get; set; }
        public string OcrCode { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; }
        public string U_FF_TIP_COM { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
    }
}
