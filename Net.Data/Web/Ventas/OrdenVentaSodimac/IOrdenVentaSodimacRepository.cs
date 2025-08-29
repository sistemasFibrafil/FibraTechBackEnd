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
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacByFiltroEntity>> GetListOrdenVentaSodimacByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacEntity>> GetOrdenVentaSodimacById(int id);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaConsultaFiltroEntity>> GetListOrdenVentaSodimacPendienteLpnByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaConsultaFiltroEntity>> GetListOrdenVentaSodimacDetallePendienteLpnByIdAndFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaConsultaFiltroEntity>> GetListOrdenVentaSodimacLpnByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaConsultaFiltroEntity>> GetListOrdenVentaSodimacDetalleById(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacEntity>> SetLpnUpdate(OrdenVentaSodimacEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetBarcodeLpnPdfById(int id);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListBarcodeEanPdfByEan(string ean);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimaConsultaFiltroEntity>> GetListOrdenVentaSodimacByFechaNumero(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListOrdenVentaSodimacExcelByFechaNumero(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<OrdenVentaSodimacSelvaByFechaNumeroEntity>> GetListOrdenVentaSodimacSelvaFechaNumero(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetListOrdenVentaSodimacSelvaPdfByFechaNumero(FilterRequestEntity value);
    }
}
