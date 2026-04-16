using System;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class PurchaseRequest1CreateRequestDto
    {
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
    }
}
