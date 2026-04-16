using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITakeInventoryFinishedProductsRepository
    {
        Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>> GetListByFilter(TakeInventoryFinishedProductsFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSummaryItemExcelByFilter(TakeInventoryFinishedProductsFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetDetailedExcelByFilter(TakeInventoryFinishedProductsFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSummaryUserExcelByFilter(TakeInventoryFinishedProductsFilterEntity value);
        Task<ResultadoTransaccionEntity<TakeInventoryFinishedProducts1Entity>> GetListByItemCode(TakeInventoryFinishedProductsModalFilterEntity value);
        Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>> GetListCurrentDate(TakeInventoryFinishedProductsFindEntity value);
        Task<ResultadoTransaccionEntity<InventoryTransferRequestQueryEntity>> GetToCopy(TakeInventoryFinishedProductsToCopyFindEntity value);
        Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>> SetCreate(TakeInventoryFinishedProductsCreateEntity value);
        Task<ResultadoTransaccionEntity<TakeInventoryFinishedProducts1Entity>> SetDeleteLine(TakeInventoryFinishedProducts1DeleteEntity value);
        Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsEntity>> SetDelete(TakeInventoryFinishedProductsDeleteEntity value);
    }
}
