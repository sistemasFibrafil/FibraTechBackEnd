using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ITiempoRepository
    {
        Task<ResultadoTransaccionEntity<AnioEntity>> GetListAnio();
        Task<ResultadoTransaccionEntity<MesEntity>> GetListMes();
        Task<ResultadoTransaccionEntity<SemanaEntity>> GetListSemana(FilterRequestEntity value);
    }
}
