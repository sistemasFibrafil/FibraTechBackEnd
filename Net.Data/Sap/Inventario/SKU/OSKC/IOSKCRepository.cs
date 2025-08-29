using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IOSKCRepository
    {
        Task<ResultadoTransaccionEntity<OSKCEntity>> SetCreate(OSKCEntity value);
        Task<ResultadoTransaccionEntity<OSKCEntity>> SetUpdate(OSKCEntity value);
        Task<ResultadoTransaccionEntity<OSKCEntity>> SetDelete(OSKCEntity value);
        Task<ResultadoTransaccionEntity<OSKCEntity>> GetListByDateRange(OSKCEntity value);
        Task<ResultadoTransaccionEntity<OSKCEntity>> GetByCode(OSKCEntity value);
        Task<ResultadoTransaccionEntity<OSKCEntity>> GetListByFiltro(OSKCEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOSKCExcel(OSKCEntity value);
    }
}
