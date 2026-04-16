using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IInvoicesRepository
    {
        Task<ResultadoTransaccionEntity<InvoicesOpenQueryEntity>> GetListOpen();
        Task<ResultadoTransaccionEntity<InvoicesQueryEntity>> GetListByFilter(InvoicesFilterEntity value);
        Task<ResultadoTransaccionEntity<InvoicesQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<InvoicesEntity>> SetCreate(InvoicesCreateEntity value);
        Task<ResultadoTransaccionEntity<InvoicesEntity>> SetUpdate(InvoicesUpdateEntity value);
        Task<ResultadoTransaccionEntity<InvoicesEntity>> SetCancel(InvoicesCancelEntity value);
    }
}
