using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
namespace Net.Data.Sap
{
    public interface ITransferenciaStockSapRepository
    {
        Task<ResultadoTransaccionEntity<MemoryStream>> GetTransferenciaStockPdfByDocEntry(int id);
    }
}
