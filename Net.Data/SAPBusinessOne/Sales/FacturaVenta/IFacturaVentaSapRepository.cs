using System.IO;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IFacturaVentaSapRepository
    {
        Task<ResultadoTransaccionResponse<VentaProyeccionSapByFechaEntity>> GetListVentaProyeccionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<FacturaVentaSapEntity>> GetListVentaResumenByFechaGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetVentaResumenExcelByFechaGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<VentaSapByFilterCodeEntity>> GetListVentaByFilter(VentaSapByFilterFindEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetVentaByFilterExcel(VentaSapByFilterFindEntity value);
        Task<ResultadoTransaccionResponse<FacturaVentaSapByFechaEntity>> GetListFacturaVentaByFilter(FacturaVentaSapByFilterFindEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetFacturaVentaByFilterExcel(FacturaVentaSapByFilterFindEntity value);
    }
}
