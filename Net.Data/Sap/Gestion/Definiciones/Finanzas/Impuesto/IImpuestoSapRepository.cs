using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IImpuestoSapRepository
    {
        Task<ResultadoTransaccionEntity<ImpuestoSapEntity>> GetListByFiltro(string filter);
        Task<ResultadoTransaccionEntity<ImpuestoSapEntity>> GetBySplCode(int slpCode);
    }
}
