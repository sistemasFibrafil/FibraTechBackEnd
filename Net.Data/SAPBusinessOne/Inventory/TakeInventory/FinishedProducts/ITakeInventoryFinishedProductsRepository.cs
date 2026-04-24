using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Query;
namespace Net.Data.SAPBusinessOne
{
    public interface ITakeInventoryFinishedProductsRepository
    {
        Task<ResultadoTransaccionResponse<TakeInventoryFinishedProductsQueryEntity>> GetListByFilter(TakeInventoryFinishedProductsFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetSummaryItemExcelByFilter(TakeInventoryFinishedProductsFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetDetailedExcelByFilter(TakeInventoryFinishedProductsFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetSummaryUserExcelByFilter(TakeInventoryFinishedProductsFilterEntity value);
        Task<ResultadoTransaccionResponse<TakeInventoryFinishedProducts1Entity>> GetListByItemCode(TakeInventoryFinishedProductsModalFilterEntity value);
        Task<ResultadoTransaccionResponse<TakeInventoryFinishedProductsQueryEntity>> GetListCurrentDate(TakeInventoryFinishedProductsFindEntity value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestQueryEntity>> GetToCopy(TakeInventoryFinishedProductsToCopyFindEntity value);
        Task<ResultadoTransaccionResponse<TakeInventoryFinishedProductsQueryEntity>> SetCreate(TakeInventoryFinishedProductsCreateEntity value);
        Task<ResultadoTransaccionResponse<TakeInventoryFinishedProducts1Entity>> SetDeleteLine(TakeInventoryFinishedProducts1DeleteEntity value);
        Task<ResultadoTransaccionResponse<TakeInventoryFinishedProductsEntity>> SetDelete(TakeInventoryFinishedProductsDeleteEntity value);
    }
}
