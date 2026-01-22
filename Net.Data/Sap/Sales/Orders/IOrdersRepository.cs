using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IOrdersRepository
    {
        Task<ResultadoTransaccionEntity<OrdersEntity>> GetListByFilter(OrdersFilterEntity value);
        Task<ResultadoTransaccionEntity<OrdersQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListSeguimientoByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListSeguimientoDetalladoDireccionFiscalByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionFiscalByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListSeguimientoDetalladoDireccionDespachoByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionDespachoByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListOrdenVentaProgramacionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenVentaProgramacionExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacSapEntity>> GetListOrdenVentaSodimacPendienteByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacSapEntity>> GetOrdenVentaSodimacPendienteById(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListOrdenVentaPreliminarPendienteByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListOrdenVentaPreliminarPendienteExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdersEntity>> SetCreate(OrdersCreateEntity value);
    }
}
