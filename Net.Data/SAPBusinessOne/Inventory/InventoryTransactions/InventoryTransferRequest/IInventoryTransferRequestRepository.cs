using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IInventoryTransferRequestRepository
    {
        Task<ResultadoTransaccionEntity<InventoryTransferRequestOpenQueryEntity>> GetListOpen();
        Task<ResultadoTransaccionEntity<InventoryTransferRequestEntity>> GetListByFilter(InventoryTransferRequestFilterEntity value);
        Task<ResultadoTransaccionEntity<InventoryTransferRequestQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<InventoryTransferRequestQueryEntity>> GetToTransferenciaByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListNotPicking();
        Task<ResultadoTransaccionEntity<InventoryTransferRequestEntity>> SetCreate(InventoryTransferRequestCreateEntity value);
        Task<ResultadoTransaccionEntity<InventoryTransferRequestEntity>> SetUpdate(InventoryTransferRequestUpdateEntity value);
        Task<ResultadoTransaccionEntity<InventoryTransferRequestEntity>> SetClose(InventoryTransferRequestCloseEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetFormatoPdfByDocEntry(int id);
    }
}
