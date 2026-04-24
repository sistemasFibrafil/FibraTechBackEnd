using System;
namespace Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Update
{
    public class PurchaseRequest1UpdateEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string? LineStatus { get; set; }

        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? LineVendor { get; set; }
        public DateTime PqtReqDate { get; set; }
        public string? AcctCode { get; set; }
        public string? OcrCode { get; set; }
        public string? WhsCode { get; set; }
        public string? UnitMsr { get; set; }
        public double Quantity { get; set; }

        public string? U_tipoOpT12 { get; set; }
        public string? U_FF_TIP_COM { get; set; }

        public int Record { get; set; } = 2;
    }
}
