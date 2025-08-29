using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ISedeSapRepository
    {
        Task<ResultadoTransaccionEntity<SedeSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<SedeSapEntity>> GetById(int id);
    }
}
