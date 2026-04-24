using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Cancel;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Cancel;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.Invoices.Cancel
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
