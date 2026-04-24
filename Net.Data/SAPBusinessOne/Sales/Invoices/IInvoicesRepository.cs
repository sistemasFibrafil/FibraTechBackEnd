using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Create;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Update;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Cancel;
namespace Net.Data.SAPBusinessOne
{
    public interface IInvoicesRepository
    {
        Task<ResultadoTransaccionResponse<InvoicesOpenQueryEntity>> GetListOpen();
        Task<ResultadoTransaccionResponse<InvoicesQueryEntity>> GetListByFilter(InvoicesFilterEntity value);
        Task<ResultadoTransaccionResponse<InvoicesQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<InvoicesEntity>> SetCreate(InvoicesCreateEntity value);
        Task<ResultadoTransaccionResponse<InvoicesEntity>> SetUpdate(InvoicesUpdateEntity value);
        Task<ResultadoTransaccionResponse<InvoicesEntity>> SetCancel(InvoicesCancelEntity value);
    }
}
