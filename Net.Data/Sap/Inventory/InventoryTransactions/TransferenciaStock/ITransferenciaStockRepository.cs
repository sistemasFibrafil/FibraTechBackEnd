using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ITransferenciaStockRepository
    {
        Task<ResultadoTransaccionEntity<TransferenciaStockQueryEntity>> GetListByFilter(TransferenciaStockFilterEntity value);
        Task<ResultadoTransaccionEntity<TransferenciaStockQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<TransferenciaStockEntity>> SetCreate(TransferenciaStockCreateEntity value);
        Task<ResultadoTransaccionEntity<TransferenciaStockEntity>> SetUpdate(TransferenciaStockUpdateEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetFormatoPdfByDocEntry(int id);
    }
}
