using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IArticuloSapRepository
    {
        Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetByCode(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetListStockGeneralByAlmacen(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetStockGeneralByAlmacenExcel(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetListStockGeneralDetalladoAlmacenByAlmacen(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetStockGeneralDetalladoAlmacenByAlmacenExcel(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloVentaByGrupoSubGrupoEstado>> GetListArticuloVentaByGrupoSubGrupoEstado(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetArticuloVentaExcelByGrupoSubGrupoEstado(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloVentaStockByGrupoSubGrupo>> GetListArticuloVentaStockByGrupoSubGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetArticuloVentaStockExcelByGrupoSubGrupo(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetListArticuloByGrupoSubGrupoFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListArticuloExcelByGrupoSubGrupoFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MovimientoStockSapByFechaSedeEntity>> GetListMovimientoStockByFechaSede(MovimientoStockSapByFechaSedeFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetMovimientoStockExcelByFechaSede(MovimientoStockSapByFechaSedeFindEntity value);
        Task<ResultadoTransaccionEntity<ArticuloSapForSodimacBySkuItemEntity>> GetArticuloForOrdenVentaSodimacBySku(ArticuloSapForSodimacBySkuEntity value);
        Task<ResultadoTransaccionEntity<ArticuloDocumentoSapEntity>> GetArticuloVentaByCode(FilterRequestEntity value);
    }
}
