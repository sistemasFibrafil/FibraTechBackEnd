using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IConductorSapRepository
    {
        Task<ResultadoTransaccionEntity<ConductorSapEntity>> GetListByFiltro(FilterRequestEntity value);
    }
}
