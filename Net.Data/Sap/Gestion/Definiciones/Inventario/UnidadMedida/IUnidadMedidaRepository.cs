using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IUnidadMedidaRepository
    {
        Task<ResultadoTransaccionEntity<UnidadMedidaEntity>> GetList();
        Task<ResultadoTransaccionEntity<UnidadMedidaEntity>> GetListByFiltro(UnidadMedidaEntity value);
    }
}
