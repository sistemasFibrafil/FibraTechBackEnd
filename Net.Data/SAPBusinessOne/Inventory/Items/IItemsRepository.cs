using System.IO;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IItemsRepository
    {
        Task<ResultadoTransaccionResponse<ItemsEntity>> GetListByFilter(ItemsFilterEntity value);
        Task<ResultadoTransaccionResponse<ItemsQueryEntity>> GetListByCode(ItemsFindByListCodeEntity value);
        Task<ResultadoTransaccionResponse<ItemsStockGeneralViewEntity>> GetListStockGeneralSummary(ItemsStockGeneralViewFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetStockGeneralSummaryExcel(ItemsStockGeneralViewFilterEntity value);
        Task<ResultadoTransaccionResponse<ItemsStockGeneralViewEntity>> GetListStockGeneralDetailed(ItemsStockGeneralViewFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetStockGeneralDetailedExcel(ItemsStockGeneralViewFilterEntity value);
        Task<ResultadoTransaccionResponse<ArticuloVentaByGrupoSubGrupoEstado>> GetListArticuloVentaByGrupoSubGrupoEstado(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetArticuloVentaExcelByGrupoSubGrupoEstado(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<ArticuloVentaStockByGrupoSubGrupo>> GetListArticuloVentaStockByGrupoSubGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetArticuloVentaStockExcelByGrupoSubGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<ArticuloReporteEntity>> GetListArticuloByGrupoSubGrupoFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetListArticuloExcelByGrupoSubGrupoFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<MovimientoStockByFechaSedeEntity>> GetListMovimientoStockByFechaSede(ArticuloMovimientoStockFindEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetMovimientoStockExcelByFechaSede(ArticuloMovimientoStockFindEntity value);
        Task<ResultadoTransaccionResponse<ArticuloForSodimacBySkuItemEntity>> GetArticuloForOrdenVentaSodimacBySku(ArticuloSodimacBySkuEntity value);
        Task<ResultadoTransaccionResponse<ArticuloDocumentoEntity>> GetArticuloVentaByCode(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<ItemsEntity>> SetCreateMassive(ItemsCreateMassiveEntity value);
    }
}
