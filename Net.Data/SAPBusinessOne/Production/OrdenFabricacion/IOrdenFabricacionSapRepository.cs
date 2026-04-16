using System.IO;
using Net.Connection;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IOrdenFabricacionSapRepository : IRepositoryBase<OrdenFabricacionSapEntity>
    {
        Task<ResultadoTransaccionEntity<OrdenFabricacionSapEntity>> GetListOrdenFabricacionBySede(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenFabricacionExcelBySede(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenFabricacionGeneralSapBySedeEntity>> GetListOrdenFabricacionGeneralBySede(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenFabricacionGeneralExcelBySede(FilterRequestEntity value);
    }
}
