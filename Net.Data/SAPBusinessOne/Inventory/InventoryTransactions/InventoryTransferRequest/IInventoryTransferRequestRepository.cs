using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Filter;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Update;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Entities;
namespace Net.Data.SAPBusinessOne
{
    public interface IInventoryTransferRequestRepository
    {
        Task<ResultadoTransaccionResponse<InventoryTransferRequestOpenQueryEntity>> GetListOpen();
        Task<ResultadoTransaccionResponse<InventoryTransferRequestEntity>> GetListByFilter(InventoryTransferRequestFilterEntity value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestQueryEntity>> GetToTransferenciaByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<PickingQueryEntity>> GetListNotPicking();
        Task<ResultadoTransaccionResponse<InventoryTransferRequestEntity>> SetCreate(InventoryTransferRequestCreateEntity value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestEntity>> SetUpdate(InventoryTransferRequestUpdateEntity value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestEntity>> SetClose(InventoryTransferRequestCloseEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetFormatoPdfByDocEntry(int id);
    }
}
