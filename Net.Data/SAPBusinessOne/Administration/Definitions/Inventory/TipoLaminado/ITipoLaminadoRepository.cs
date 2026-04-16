using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITipoLaminadoRepository
    {
        Task<ResultadoTransaccionEntity<TipoLaminadoEntity>> GetList();
        Task<ResultadoTransaccionEntity<TipoLaminadoEntity>> GetListByFiltro(TipoLaminadoEntity value);
    }
}
