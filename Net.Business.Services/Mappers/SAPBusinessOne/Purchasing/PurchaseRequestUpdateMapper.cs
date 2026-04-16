using System.Linq;
using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class PurchaseRequestUpdateMapper
    {
        public static PurchaseRequestUpdateEntity ToEntity(PurchaseRequestUpdateRequestDto dto)
        {
            return new PurchaseRequestUpdateEntity()
            {
                DocEntry = dto.DocEntry,

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
                
                U_UsrUpdate = dto.U_UsrUpdate,

                Lines = [.. dto.Lines.Select(l => new PurchaseRequest1UpdateEntity
                {
                    DocEntry = l.DocEntry,
                    LineNum = l.LineNum,
                    LineStatus = l.LineStatus,

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
                    U_FF_TIP_COM = l.U_FF_TIP_COM,

                    Record = l.Record
                })]

            };
        }
    }
}
