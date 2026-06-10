using System;
namespace Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Validate
{
    public class PurchaseRequestLinesItemsValidateEntity
    {
        public string? ItemCode { get; set; }

        public string? LineVendor { get; set; }
        public DateTime PqtReqDate { get; set; }
        public string? FormatCode { get; set; }
        public string? OcrCode { get; set; }

        public string? WhsCode { get; set; }

        public string? UnitMsr { get; set; }
        public decimal Quantity { get; set; }

        public string? U_tipoOpT12 { get; set; }
        public string? U_FF_TIP_COM { get; set; }
    }
}
