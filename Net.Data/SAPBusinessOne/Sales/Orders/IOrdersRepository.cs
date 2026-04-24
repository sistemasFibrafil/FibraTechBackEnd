using System.IO;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Sales.Orders.Close;
using Net.Business.Entities.SAPBusinessOne.Sales.Orders.Create;
using Net.Business.Entities.SAPBusinessOne.Sales.Orders.Update;
namespace Net.Data.SAPBusinessOne
{
    public interface IOrdersRepository
    {
        Task<ResultadoTransaccionResponse<OrdersOpenQueryEntity>> GetListOpen();
        Task<ResultadoTransaccionResponse<OrdersQueryEntity>> GetListByFilter(OrdersFilterEntity value);
        Task<ResultadoTransaccionResponse<OrdersQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<OrdersQueryEntity>> GetToCopy(int docEntry);
        Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListSeguimientoByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetSeguimientoByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListSeguimientoDetalladoDireccionFiscalByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetSeguimientoDetalladoDireccionFiscalByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListSeguimientoDetalladoDireccionDespachoByFilter(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetSeguimientoDetalladoDireccionDespachoByFilterExcel(OrdersSeguimientoFindEntity value);
        Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListOrdenVentaProgramacionByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetOrdenVentaProgramacionExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdersSodimacQueryEntity>> GetListOrdenVentaSodimacPendienteByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdersSodimacQueryEntity>> GetOrdenVentaSodimacPendienteById(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListOrdenVentaPreliminarPendienteByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetListOrdenVentaPreliminarPendienteExcelByFecha(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdersEntity>> SetCreate(OrdersCreateEntity value);
        Task<ResultadoTransaccionResponse<OrdersEntity>> SetUpdate(OrdersUpdateEntity value);
        Task<ResultadoTransaccionResponse<OrdersEntity>> SetClose(OrdersCloseEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintNationalDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintExportPlantaDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintExportClienteDocEntry(int docEntry);
    }
}
