using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IPickingRepository
    {
        Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListByFilter(PickingFilterEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> GetListByBaseEntry(PickingFindEntity value);
        Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListByBaseEntryBaseType(PickingFindEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> GetListByTarget(PickingEntity value);
        Task<ResultadoTransaccionEntity<InventoryTransferRequestQueryEntity>> GetToCopyTransferRequest(PickingCopyToFindEntity value);
        Task<ResultadoTransaccionEntity<OrdersQueryEntity>> GetToCopyOrder(PickingCopyToFindEntity value);
        Task<ResultadoTransaccionEntity<InvoicesQueryEntity>> GetToCopyInvoice(PickingCopyToFindEntity value);
        Task<ResultadoTransaccionEntity<PickingQueryEntity>> SetCreate(PickingEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> SetRelease(PickingEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> SetDelete(PickingEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> SetDeleteMassive(PickingEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetPickingPrint(PickingEntity value);
    }
}
