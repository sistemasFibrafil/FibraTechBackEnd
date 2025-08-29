using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ITablaDefinidaUsuarioSapRepository
    {
        Task<ResultadoTransaccionEntity<TablaDefinidaUsuarioSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<TablaDefinidaUsuarioSapEntity>> GetByCode(FilterRequestEntity value);
    }
}
