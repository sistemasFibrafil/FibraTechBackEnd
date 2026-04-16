using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class InvoicesCancelMapper
    {
        public static InvoicesCancelEntity ToEntity(InvoicesCancelRequestDto dto)
        {
            return new InvoicesCancelEntity
            {
                DocEntry = dto.DocEntry,
                U_UsrCreate = dto.U_UsrCreate,
                U_UsrCancel = dto.U_UsrCancel
            };
        }
    }
}
