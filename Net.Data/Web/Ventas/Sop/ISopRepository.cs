using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ISopRepository
    {
        Task<ResultadoTransaccionEntity<SopEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<SopEntity>> GetById(int id);
        Task<ResultadoTransaccionEntity<SopEntity>> SetCreate(SopEntity value);
        Task<ResultadoTransaccionEntity<SopEntity>> SetUpdate(SopEntity value);
        Task<ResultadoTransaccionEntity<SopEntity>> SetDelete(SopEntity value);
        Task<ResultadoTransaccionEntity<SopDetalleEntity>> SetDeleteDetalle(SopDetalleEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSopExcelById(FilterRequestEntity value);
    }
}
