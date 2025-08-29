using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ITipoOperacionSapRepository
    {
        Task<ResultadoTransaccionEntity<TipoOperacionSapEntity>> GetListByFiltro(TipoOperacionSapEntity value);
    }
}
