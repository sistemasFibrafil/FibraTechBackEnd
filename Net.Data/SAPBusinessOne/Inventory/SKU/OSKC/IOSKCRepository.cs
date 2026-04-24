using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IOSKCRepository
    {
        Task<ResultadoTransaccionResponse<OSKCEntity>> SetCreate(OSKCEntity value);
        Task<ResultadoTransaccionResponse<OSKCEntity>> SetUpdate(OSKCEntity value);
        Task<ResultadoTransaccionResponse<OSKCEntity>> SetDelete(OSKCEntity value);
        Task<ResultadoTransaccionResponse<OSKCEntity>> GetListByDateRange(OSKCEntity value);
        Task<ResultadoTransaccionResponse<OSKCEntity>> GetByCode(OSKCEntity value);
        Task<ResultadoTransaccionResponse<OSKCEntity>> GetListByFiltro(OSKCEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetOSKCExcel(OSKCEntity value);
    }
}
