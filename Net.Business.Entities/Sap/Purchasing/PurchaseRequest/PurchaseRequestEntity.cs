using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    /// <summary>
    /// Solicitud de compra
    /// </summary>
    public class PurchaseRequestEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; }
        public string DocStatus { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime ReqDate { get; set; }
        public int ReqType { get; set; }
        public string Requester { get; set; }
        public string ReqName { get; set; }
        public Int16? Branch { get; set; }
        public Int16? Department { get; set; }
        public string Notify { get; set; }
        public string Email { get; set; }
        public string DocType { get; set; }
        public int? OwnerCode { get; set; }
        public string Comments { get; set; }


        // 🔗 1 → N (OPRQ → PRQ1)
        public ICollection<PurchaseRequest1Entity> Lines { get; set; } = new List<PurchaseRequest1Entity>();
    }

    public class PurchaseRequest1Entity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ObjType { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string LineVendor { get; set; }
        public DateTime PQTReqDate { get; set; }
        public string AcctCode { get; set; }
        public string OcrCode { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; }
        public string U_FF_TIP_COM { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }


        // 🔗 N → 1 (PRQ1 → ChartOfAccounts)
        public ChartOfAccountsEntity ChartOfAccounts { get; set; }


        // 🔗 N → 1 (PRQ1 → TipoOperacion)
        public TipoOperacionEntity TipoOperacion { get; set; }
    }
}
