using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Find;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Filter;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Query;
namespace Net.Data.SAPBusinessOne
{
    public interface IPickingRepository
    {
        Task<ResultadoTransaccionResponse<PickingQueryEntity>> GetListByFilter(PickingFilterEntity value);
        Task<ResultadoTransaccionResponse<PickingEntity>> GetListByBaseEntry(PickingFindEntity value);
        Task<ResultadoTransaccionResponse<PickingQueryEntity>> GetListByBaseEntryBaseType(PickingFindEntity value);
        Task<ResultadoTransaccionResponse<PickingEntity>> GetListByTarget(PickingEntity value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestQueryEntity>> GetToCopyTransferRequest(PickingCopyToFindEntity value);
        Task<ResultadoTransaccionResponse<OrdersQueryEntity>> GetToCopyOrder(PickingCopyToFindEntity value);
        Task<ResultadoTransaccionResponse<InvoicesQueryEntity>> GetToCopyInvoice(PickingCopyToFindEntity value);
        Task<ResultadoTransaccionResponse<PickingQueryEntity>> SetCreate(PickingEntity value);
        Task<ResultadoTransaccionResponse<PickingEntity>> SetRelease(PickingEntity value);
        Task<ResultadoTransaccionResponse<PickingEntity>> SetDelete(PickingEntity value);
        Task<ResultadoTransaccionResponse<PickingEntity>> SetDeleteMassive(PickingEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetPickingPrint(PickingEntity value);
    }
}
