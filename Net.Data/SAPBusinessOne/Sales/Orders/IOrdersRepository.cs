using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IOrdersRepository
    {
        Task<ResultadoTransaccionEntity<OrdersOpenQueryEntity>> GetListOpen();
        Task<ResultadoTransaccionEntity<OrdersQueryEntity>> GetListByFilter(OrdersFilterEntity value);
        Task<ResultadoTransaccionEntity<OrdersQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<OrdersQueryEntity>> GetToCopy(int docEntry);
        Task<ResultadoTransaccionEntity<OrdersFechaQueryEntity>> GetListSeguimientoByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaQueryEntity>> GetListSeguimientoDetalladoDireccionFiscalByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionFiscalByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaQueryEntity>> GetListSeguimientoDetalladoDireccionDespachoByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionDespachoByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaQueryEntity>> GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaQueryEntity>> GetListOrdenVentaProgramacionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenVentaProgramacionExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdersSodimacQueryEntity>> GetListOrdenVentaSodimacPendienteByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdersSodimacQueryEntity>> GetOrdenVentaSodimacPendienteById(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdersFechaQueryEntity>> GetListOrdenVentaPreliminarPendienteByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListOrdenVentaPreliminarPendienteExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdersEntity>> SetCreate(OrdersCreateEntity value);
        Task<ResultadoTransaccionEntity<OrdersEntity>> SetUpdate(OrdersUpdateEntity value);
        Task<ResultadoTransaccionEntity<OrdersEntity>> SetClose(OrdersCloseEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetPrintNationalDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetPrintExportPlantaDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetPrintExportClienteDocEntry(int docEntry);
    }
}
