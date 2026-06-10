using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
namespace Net.Business.Logic.Mappers.SAPBusinessOne.Purchasing.PurchaseRequest.Close
{
    public class PurchaseRequestCloseMapper
    {
        public static PurchaseRequestCloseEntity ToEntity(PurchaseRequestCloseRequestDto dto)
        {
            return new PurchaseRequestCloseEntity()
            {
                DocEntry = dto.DocEntry,
                U_UsrClose = dto.U_UsrClose,
            };
        }
    }
}
