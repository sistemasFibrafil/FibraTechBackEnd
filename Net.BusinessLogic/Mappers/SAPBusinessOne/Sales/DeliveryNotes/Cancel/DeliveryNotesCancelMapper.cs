using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.DeliveryNotes.Cancel
{
    public class DeliveryNotesCancelMapper
    {
        public static DeliveryNotesCancelEntity ToEntity(DeliveryNotesCancelRequestDto value)
        {
            return new DeliveryNotesCancelEntity
            {
                DocEntry = value.DocEntry,
                U_UsrCreate = value.U_UsrCreate,
                U_UsrCancel = value.U_UsrCancel
            };
        }
    }
}
