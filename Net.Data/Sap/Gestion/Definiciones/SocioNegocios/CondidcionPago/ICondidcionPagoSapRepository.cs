using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ICondidcionPagoSapRepository
    {
        Task<ResultadoTransaccionEntity<CondicionPagoSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<CondicionPagoSapEntity>> GetById(int id);
    }
}
