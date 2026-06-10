using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Business.Logic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Validate
{
    public class PurchaseRequestLinesServicesValidateMapper
    {
        public static List<PurchaseRequestLinesServicesValidateEntity> ToEntity(List<PurchaseRequestLinesServicesValidateRequestDto> dto)
        {
            return [.. dto.Select(l => new PurchaseRequestLinesServicesValidateEntity
            {
                Dscription = l.Dscription,
                LineVendor = l.LineVendor,
                PqtReqDate = l.PqtReqDate,
                FormatCode = l.FormatCode,
                OcrCode = l.OcrCode,
                U_tipoOpT12 = l.U_tipoOpT12,
                U_FF_TIP_COM = l.U_FF_TIP_COM
            })];
        }
    }
}
