using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Data.SAPBusinessOne
{
    public interface IPurchaseRequestRepository
    {
        #region <<< CONSULTAS >>>

        Task<ResultadoTransaccionResponse<PurchaseRequestQueryEntity>> GetListByFilter(PurchaseRequestFilterEntity value);
        Task<ResultadoTransaccionResponse<PurchaseRequestQueryEntity>> GetByDocEntry(int docEntry);

        #endregion


        #region <<< OPERACIONES >>>

        Task<ResultadoTransaccionResponse<PurchaseRequestLinesQueryEntity>> SetValidateLinesItemsExcel(List<PurchaseRequestLinesItemsValidateEntity> Lines);
        Task<ResultadoTransaccionResponse<PurchaseRequestLinesQueryEntity>> SetValidateLinesServicsExcel(List<PurchaseRequestLinesServicesValidateEntity> value);
        Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetCreate(PurchaseRequestCreateEntity value);
        Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetUpdate(PurchaseRequestUpdateEntity value);
        Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetClose(PurchaseRequestCloseEntity value);

        #endregion


        #region <<< EXPORTACIONES >>>

        Task<ResultadoTransaccionResponse<MemoryStream>> GetDownloadItemsTemplate();

        Task<ResultadoTransaccionResponse<MemoryStream>> GetDownloadServicesTemplate();

        #endregion
    }
}
