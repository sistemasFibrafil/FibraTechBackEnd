using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Business.Logic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Validate
{
    public class PurchaseRequestLinesItemsValidateMapper
    {
        public static List<PurchaseRequestLinesItemsValidateEntity> ToEntity(List<PurchaseRequestLinesItemsValidateRequestDto> dto)
        {
            return [.. dto.Select(l => new PurchaseRequestLinesItemsValidateEntity
            {
                ItemCode = l.ItemCode,
                LineVendor = l.LineVendor,
                PqtReqDate = l.PqtReqDate,
                FormatCode = l.FormatCode,
                OcrCode = l.OcrCode,
                WhsCode = l.WhsCode,
                UnitMsr = l.UnitMsr,
                Quantity = l.Quantity,
                U_tipoOpT12 = l.U_tipoOpT12,
                U_FF_TIP_COM = l.U_FF_TIP_COM
            })];
        }
    }
}
