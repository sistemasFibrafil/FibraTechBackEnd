using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IItemsRepository
    {
        Task<ResultadoTransaccionEntity<ItemsEntity>> GetListByFilter(ItemsFilterEntity value);
        Task<ResultadoTransaccionEntity<ItemsQueryEntity>> GetListByCode(ItemsFindByListCodeEntity value);
        Task<ResultadoTransaccionEntity<ItemsStockGeneralViewEntity>> GetListStockGeneralSummary(ItemsStockGeneralViewFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetStockGeneralSummaryExcel(ItemsStockGeneralViewFilterEntity value);
        Task<ResultadoTransaccionEntity<ItemsStockGeneralViewEntity>> GetListStockGeneralDetailed(ItemsStockGeneralViewFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetStockGeneralDetailedExcel(ItemsStockGeneralViewFilterEntity value);
        Task<ResultadoTransaccionEntity<ArticuloVentaByGrupoSubGrupoEstado>> GetListArticuloVentaByGrupoSubGrupoEstado(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetArticuloVentaExcelByGrupoSubGrupoEstado(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloVentaStockByGrupoSubGrupo>> GetListArticuloVentaStockByGrupoSubGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetArticuloVentaStockExcelByGrupoSubGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloReporteEntity>> GetListArticuloByGrupoSubGrupoFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListArticuloExcelByGrupoSubGrupoFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MovimientoStockByFechaSedeEntity>> GetListMovimientoStockByFechaSede(ArticuloMovimientoStockFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetMovimientoStockExcelByFechaSede(ArticuloMovimientoStockFindEntity value);
        Task<ResultadoTransaccionEntity<ArticuloForSodimacBySkuItemEntity>> GetArticuloForOrdenVentaSodimacBySku(ArticuloSodimacBySkuEntity value);
        Task<ResultadoTransaccionEntity<ArticuloDocumentoEntity>> GetArticuloVentaByCode(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ItemsEntity>> SetCreateMassive(ItemsCreateMassiveEntity value);
    }
}
