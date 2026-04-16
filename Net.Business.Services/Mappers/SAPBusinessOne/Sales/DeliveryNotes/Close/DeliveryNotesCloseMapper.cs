using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
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
