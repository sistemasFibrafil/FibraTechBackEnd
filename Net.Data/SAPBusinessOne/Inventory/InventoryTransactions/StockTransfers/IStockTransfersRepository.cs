using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Filter;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Update;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Entities;
namespace Net.Data.SAPBusinessOne
{
    public interface IStockTransfersRepository
    {
        Task<ResultadoTransaccionResponse<TransferenciaStockQueryEntity>> GetListByFilter(TransferenciaStockFilterEntity value);
        Task<ResultadoTransaccionResponse<TransferenciaStockQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<StockTransfersEntity>> SetCreate(StockTransfersCreateEntity value);
        Task<ResultadoTransaccionResponse<StockTransfersEntity>> SetUpdate(StockTransfersUpdateEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetFormatoPdfByDocEntry(int id);
    }
}
