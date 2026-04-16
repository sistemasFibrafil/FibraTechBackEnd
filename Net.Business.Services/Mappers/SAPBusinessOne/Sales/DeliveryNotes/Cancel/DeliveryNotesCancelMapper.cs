using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
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
