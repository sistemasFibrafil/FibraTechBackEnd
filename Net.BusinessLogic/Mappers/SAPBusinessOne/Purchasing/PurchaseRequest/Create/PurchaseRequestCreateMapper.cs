using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Create
{
    public class PurchaseRequestCreateMapper
    {
        public static PurchaseRequestCreateEntity ToEntity(PurchaseRequestCreateRequestDto dto)
        {
            return new PurchaseRequestCreateEntity()
            {
                DocDate = dto.DocDate,
                DocDueDate = dto.DocDueDate,
                TaxDate = dto.TaxDate,
                ReqDate = dto.ReqDate,

                DocType = dto.DocType,

                ReqType = dto.ReqType,
                Requester = dto.Requester,
                ReqName = dto.ReqName,

                Branch = dto.Branch,
                Department = dto.Department,

                Notify = dto.Notify,
                Email = dto.Email,

                OwnerCode = dto.OwnerCode,

                Comments = dto.Comments,

                U_UsrCreate = dto.U_UsrCreate,

                Lines = [.. dto.Lines.Select(l => new PurchaseRequest1CreateEntity
                {
                    ItemCode = l.ItemCode,
                    Dscription = l.Dscription,

                    LineVendor = l.LineVendor,
                    PqtReqDate = l.PqtReqDate,

                    AcctCode = l.AcctCode,
                    OcrCode = l.OcrCode,

                    WhsCode = l.WhsCode,

                    UnitMsr = l.UnitMsr,
                    Quantity = l.Quantity,

                    U_tipoOpT12 = l.U_tipoOpT12,
                    U_FF_TIP_COM = l.U_FF_TIP_COM
                })]
            };
        }
    }
}
