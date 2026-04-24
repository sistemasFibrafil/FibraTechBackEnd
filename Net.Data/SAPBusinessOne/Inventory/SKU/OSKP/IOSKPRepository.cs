using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IOSKPRepository
    {
        Task<ResultadoTransaccionResponse<OSKPEntity>> SetCreate(OSKPEntity value);
        Task<ResultadoTransaccionResponse<OSKPEntity>> SetUpdate(OSKPEntity value);
        Task<ResultadoTransaccionResponse<OSKPEntity>> SetDelete(OSKPEntity value);
        Task<ResultadoTransaccionResponse<OSKPEntity>> GetListByFiltro(OSKPEntity value);
        Task<ResultadoTransaccionResponse<OSKPEntity>> GetByDocEntry(OSKPEntity value);
    }
}
