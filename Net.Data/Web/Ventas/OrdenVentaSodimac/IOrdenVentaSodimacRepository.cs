using System.IO;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IOrdenVentaSodimacRepository
    {
        Task<ResultadoTransaccionResponse<OrdenVentaSodimacEntity>> SetCreate(OrdenVentaSodimacEntity value);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimacEntity>> SetUpdate(OrdenVentaSodimacEntity value);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimacQueryEntity>> GetListOrdenVentaSodimacByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimacEntity>> GetOrdenVentaSodimacById(int id);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacPendienteLpnByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacDetallePendienteLpnByIdAndFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacLpnByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacDetalleById(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimacEntity>> SetLpnUpdate(OrdenVentaSodimacEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetBarcodeLpnPdfById(int id);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetListBarcodeEanPdfByEan(string ean);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimaGeneralQueryEntity>> GetListOrdenVentaSodimacByFechaNumero(OrdenVentaSodimacFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetListOrdenVentaSodimacExcelByFechaNumero(OrdenVentaSodimacFilterEntity value);
        Task<ResultadoTransaccionResponse<OrdenVentaSodimacSelvaQueryEntity>> GetListOrdenVentaSodimacSelvaFechaNumero(OrdenVentaSodimacSelvaFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetListOrdenVentaSodimacSelvaPdfByFechaNumero(OrdenVentaSodimacSelvaFilterEntity value);
    }
}
