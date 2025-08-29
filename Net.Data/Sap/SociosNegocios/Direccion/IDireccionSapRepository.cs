using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IDireccionSapRepository
    {
        Task<ResultadoTransaccionEntity<DireccionSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<DireccionSapEntity>> GetByCode(FilterRequestEntity value);
    }
}
