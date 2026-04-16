using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IOrdenVentaSodimacRepository
    {
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacEntity>> SetCreate(OrdenVentaSodimacEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacEntity>> SetUpdate(OrdenVentaSodimacEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacQueryEntity>> GetListOrdenVentaSodimacByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacEntity>> GetOrdenVentaSodimacById(int id);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacPendienteLpnByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacDetallePendienteLpnByIdAndFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacLpnByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacDetalleById(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacEntity>> SetLpnUpdate(OrdenVentaSodimacEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetBarcodeLpnPdfById(int id);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListBarcodeEanPdfByEan(string ean);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacByFechaNumero(OrdenVentaSodimacFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListOrdenVentaSodimacExcelByFechaNumero(OrdenVentaSodimacFilterEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacSelvaQueryEntity>> GetListOrdenVentaSodimacSelvaFechaNumero(OrdenVentaSodimacSelvaFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListOrdenVentaSodimacSelvaPdfByFechaNumero(OrdenVentaSodimacSelvaFilterEntity value);
    }
}
