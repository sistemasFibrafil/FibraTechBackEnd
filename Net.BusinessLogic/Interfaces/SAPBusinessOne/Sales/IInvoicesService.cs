using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Cancel;
using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Create;
using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Update;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.Sales
{
    public interface IInvoicesService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(InvoicesCreateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetUpdate(InvoicesUpdateRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetCancel(InvoicesCancelRequestDto dto);
    }
}
