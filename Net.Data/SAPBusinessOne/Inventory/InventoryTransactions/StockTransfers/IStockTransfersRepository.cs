using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IStockTransfersRepository
    {
        Task<ResultadoTransaccionEntity<TransferenciaStockQueryEntity>> GetListByFilter(TransferenciaStockFilterEntity value);
        Task<ResultadoTransaccionEntity<TransferenciaStockQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<StockTransfersEntity>> SetCreate(StockTransfersCreateEntity value);
        Task<ResultadoTransaccionEntity<StockTransfersEntity>> SetUpdate(StockTransfersUpdateEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetFormatoPdfByDocEntry(int id);
    }
}
