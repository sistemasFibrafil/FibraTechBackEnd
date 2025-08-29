using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ITipoLaminadoRepository
    {
        Task<ResultadoTransaccionEntity<TipoLaminadoEntity>> GetList();
        Task<ResultadoTransaccionEntity<TipoLaminadoEntity>> GetListByFiltro(TipoLaminadoEntity value);
    }
}
