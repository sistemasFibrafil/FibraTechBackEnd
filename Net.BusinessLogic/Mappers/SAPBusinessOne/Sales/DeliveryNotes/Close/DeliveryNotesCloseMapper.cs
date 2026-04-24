using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Close;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Close;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.DeliveryNotes.Close
{
    public class DeliveryNotesCloseMapper
    {
        public static DeliveryNotesCloseEntity ToEntity(DeliveryNotesCloseRequestDto dto)
        {
            return new DeliveryNotesCloseEntity
            {
                DocEntry = dto.DocEntry,
                U_UsrClose = dto.U_UsrClose
            };
        }
    }
}
