using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IUnidadMedidaRepository
    {
        Task<ResultadoTransaccionEntity<UnidadMedidaEntity>> GetList();
        Task<ResultadoTransaccionEntity<UnidadMedidaEntity>> GetListByFiltro(UnidadMedidaEntity value);
    }
}
