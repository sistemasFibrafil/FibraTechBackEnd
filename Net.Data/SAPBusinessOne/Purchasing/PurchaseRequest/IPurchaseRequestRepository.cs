using System.IO;
using System.Threading.Tasks;
using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IPurchaseRequestRepository
    {
        Task<ResultadoTransaccionEntity<PurchaseRequestQueryEntity>> GetListByFilter(PurchaseRequestFilterEntity value);
        Task<ResultadoTransaccionEntity<PurchaseRequestQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetDownloadFormat();
        Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetCreate(PurchaseRequestCreateEntity value);
        Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetUpdate(PurchaseRequestUpdateEntity value);
        Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetClose(PurchaseRequestCloseEntity value);
    }
}
