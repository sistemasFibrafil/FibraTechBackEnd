using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ITipoDocumentoRepository
    {
        Task<ResultadoTransaccionEntity<TipoDocumentoEntity>> GetListByFiltro(FilterRequestEntity value);
    }
}
