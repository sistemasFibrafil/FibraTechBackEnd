using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITipoLaminadoRepository
    {
        Task<ResultadoTransaccionResponse<TipoLaminadoEntity>> GetList();
        Task<ResultadoTransaccionResponse<TipoLaminadoEntity>> GetListByFiltro(TipoLaminadoEntity value);
    }
}
