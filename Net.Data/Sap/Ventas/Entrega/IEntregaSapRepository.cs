using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.IO;
namespace Net.Data.Sap
{
    public interface IEntregaSapRepository
    {
        Task<ResultadoTransaccionEntity<EntregaSapByFechaEntity>> GetListGuiaByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetGuiaExcelByFecha(FilterRequestEntity value);
    }
}
