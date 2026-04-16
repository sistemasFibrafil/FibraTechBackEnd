using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class PurchaseRequest1QueryEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ObjType { get; set; } = string.Empty;
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public string LineStatus { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string? Dscription { get; set; }
        public string? LineVendor { get; set; }
        public DateTime PqtReqDate { get; set; }
        public string? AcctCode { get; set; }
        public string? FormatCode { get; set; }
        public string? AcctName { get; set; }
        public string? OcrCode { get; set; }
        public string WhsCode { get; set; } = string.Empty;
        public string? U_tipoOpT12 { get; set; }
        public string? U_tipoOpT12Nam { get; set; }
        public string? U_FF_TIP_COM { get; set; }
        public string? U_FF_TIP_COM_NAM { get; set; }
        public string? UnitMsr { get; set; }
        public decimal OnHand { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public int Record { get; set; } = 2;
    }
}
