using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
namespace Net.Data.SAPBusinessOne
{
    public interface IPurchaseRequestRepository
    {
        Task<ResultadoTransaccionResponse<PurchaseRequestQueryEntity>> GetListByFilter(PurchaseRequestFilterEntity value);
        Task<ResultadoTransaccionResponse<PurchaseRequestQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetDownloadFormat();
        Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetCreate(PurchaseRequestCreateEntity value);
        Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetUpdate(PurchaseRequestUpdateEntity value);
        Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetClose(PurchaseRequestCloseEntity value);
    }
}
