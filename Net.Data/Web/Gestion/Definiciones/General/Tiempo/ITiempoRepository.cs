using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ITiempoRepository
    {
        Task<ResultadoTransaccionResponse<AnioEntity>> GetListAnio();
        Task<ResultadoTransaccionResponse<MesEntity>> GetListMes();
        Task<ResultadoTransaccionResponse<SemanaEntity>> GetListSemana(FilterRequestEntity value);
    }
}
