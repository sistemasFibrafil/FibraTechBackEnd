using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IOSKPRepository
    {
        Task<ResultadoTransaccionEntity<OSKPEntity>> SetCreate(OSKPEntity value);
        Task<ResultadoTransaccionEntity<OSKPEntity>> SetUpdate(OSKPEntity value);
        Task<ResultadoTransaccionEntity<OSKPEntity>> SetDelete(OSKPEntity value);
        Task<ResultadoTransaccionEntity<OSKPEntity>> GetListByFiltro(OSKPEntity value);
        Task<ResultadoTransaccionEntity<OSKPEntity>> GetByDocEntry(OSKPEntity value);
    }
}
