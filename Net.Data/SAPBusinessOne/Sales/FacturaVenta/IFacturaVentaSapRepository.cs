using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IFacturaVentaSapRepository
    {
        Task<ResultadoTransaccionEntity<VentaProyeccionSapByFechaEntity>> GetListVentaProyeccionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<FacturaVentaSapEntity>> GetListVentaResumenByFechaGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetVentaResumenExcelByFechaGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<VentaSapByFilterCodeEntity>> GetListVentaByFilter(VentaSapByFilterFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetVentaByFilterExcel(VentaSapByFilterFindEntity value);
        Task<ResultadoTransaccionEntity<FacturaVentaSapByFechaEntity>> GetListFacturaVentaByFilter(FacturaVentaSapByFilterFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetFacturaVentaByFilterExcel(FacturaVentaSapByFilterFindEntity value);
    }
}
