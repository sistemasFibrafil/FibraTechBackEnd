using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ISocioNegocioSapRepository
    {
        Task<ResultadoTransaccionEntity<SocioNegocioSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<SocioNegocioSapEntity>> GetByCardCode(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<SocioNegocioSapEntity>> GetLitClienteBySectorEstado(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetClienteExcelBySectorEstado(FilterRequestEntity value);
    }
}
