using System;
namespace Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate
{
    public class PurchaseRequestLinesServicesValidateRequestDto
    {
        public string? Dscription { get; set; }

        public string? LineVendor { get; set; }
        public DateTime PqtReqDate { get; set; }
        public string? FormatCode { get; set; }
        public string? OcrCode { get; set; }

        public string? U_tipoOpT12 { get; set; }
        public string? U_FF_TIP_COM { get; set; }
    }
}
