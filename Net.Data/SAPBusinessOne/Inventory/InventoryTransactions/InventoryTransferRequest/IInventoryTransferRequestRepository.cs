using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Filter;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Update;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Entities;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate;
namespace Net.Data.SAPBusinessOne
{
    public interface IInventoryTransferRequestRepository
    {
        #region <<< CONSULTAS >>>

        Task<ResultadoTransaccionResponse<InventoryTransferRequestOpenQueryEntity>> GetListOpen();
        Task<ResultadoTransaccionResponse<InventoryTransferRequestEntity>> GetListByFilter(InventoryTransferRequestFilterEntity value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestQueryEntity>> GetToTransferenciaByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<PickingQueryEntity>> GetListNotPicking();

        #endregion


        #region <<< OPERACIONES >>>

        Task<ResultadoTransaccionResponse<InventoryTransferRequestLinesQueryEntity>> SetValidateLinesExcel(List<InventoryTransferRequestLinesValidateEntity> value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestEntity>> SetCreate(InventoryTransferRequestCreateEntity value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestEntity>> SetUpdate(InventoryTransferRequestUpdateEntity value);
        Task<ResultadoTransaccionResponse<InventoryTransferRequestEntity>> SetClose(InventoryTransferRequestCloseEntity value);

        #endregion


        #region <<< EXPORTACIONES >>>

        Task<ResultadoTransaccionResponse<MemoryStream>> GetDownloadItemsTemplate();

        #endregion


        #region <<< IMPRESIONES >>>

        Task<ResultadoTransaccionResponse<MemoryStream>> GetFormatoPdfByDocEntry(int docEntry);

        #endregion
    }
}
