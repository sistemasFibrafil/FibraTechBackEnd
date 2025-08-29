using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IOrdenVentaSapRepository
    {
        Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListSeguimientoByFilter(OrdenVentaSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoByFilterExcel(OrdenVentaSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListSeguimientoDetalladoDireccionFiscalByFilter(OrdenVentaSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionFiscalByFilterExcel(OrdenVentaSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListSeguimientoDetalladoDireccionDespachoByFilter(OrdenVentaSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionDespachoByFilterExcel(OrdenVentaSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListOrdenVentaProgramacionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenVentaProgramacionExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacSapEntity>> GetListOrdenVentaSodimacPendienteByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacSapEntity>> GetOrdenVentaSodimacPendienteById(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListOrdenVentaPreliminarPendienteByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListOrdenVentaPreliminarPendienteExcelByFecha(FilterRequestEntity value);
    }
}
