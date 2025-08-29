using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ISerieNumeracionRepository
    {
        Task<ResultadoTransaccionEntity<SerieNumeracionEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<SerieNumeracionEntity>> SetAction(SerieNumeracionEntity value);
    }
}
