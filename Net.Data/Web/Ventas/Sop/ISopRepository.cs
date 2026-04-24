using System.IO;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ISopRepository
    {
        Task<ResultadoTransaccionResponse<SopEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<SopEntity>> GetById(int id);
        Task<ResultadoTransaccionResponse<SopEntity>> SetCreate(SopEntity value);
        Task<ResultadoTransaccionResponse<SopEntity>> SetUpdate(SopEntity value);
        Task<ResultadoTransaccionResponse<SopEntity>> SetDelete(SopEntity value);
        Task<ResultadoTransaccionResponse<SopDetalleEntity>> SetDeleteDetalle(SopDetalleEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetSopExcelById(FilterRequestEntity value);
    }
}
