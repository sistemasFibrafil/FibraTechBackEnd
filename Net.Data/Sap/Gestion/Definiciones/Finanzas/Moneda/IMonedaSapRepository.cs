using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IMonedaSapRepository
    {
        Task<ResultadoTransaccionEntity<MonedaSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MonedaSapEntity>> GetByCode(FilterRequestEntity value);
    }
}
