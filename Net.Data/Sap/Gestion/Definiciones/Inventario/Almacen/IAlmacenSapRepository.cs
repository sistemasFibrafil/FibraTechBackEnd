using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IAlmacenSapRepository
    {
        Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetListAlmacenProduccion();
        Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetByCode(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetListByEstado(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloAlmacenSapEntity>> GetListByWhsCodeAndItemCode(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetExisteByCodeAndSede(FilterRequestEntity value);
    }
}
