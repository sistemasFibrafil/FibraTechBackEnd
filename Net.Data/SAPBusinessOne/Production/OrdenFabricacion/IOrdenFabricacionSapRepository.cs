using System.IO;
using Net.Connection;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IOrdenFabricacionSapRepository : IRepositoryBase<OrdenFabricacionSapEntity>
    {
        Task<ResultadoTransaccionResponse<OrdenFabricacionSapEntity>> GetListOrdenFabricacionBySede(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetOrdenFabricacionExcelBySede(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdenFabricacionGeneralSapBySedeEntity>> GetListOrdenFabricacionGeneralBySede(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetOrdenFabricacionGeneralExcelBySede(FilterRequestEntity value);
    }
}
