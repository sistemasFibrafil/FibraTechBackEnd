using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IPurchaseRequestRepository
    {
        Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> GetListByFilter(PurchaseRequestFilterEntity value);
        Task<ResultadoTransaccionEntity<PurchaseRequestQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetCreate(PurchaseRequestCreateEntity value);
        Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetUpdate(PurchaseRequestUpdateEntity value);
        Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetClose(PurchaseRequestCloseEntity value);
    }
}
