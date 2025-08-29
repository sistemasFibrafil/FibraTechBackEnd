using System.Threading.Tasks;
using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IDocumentoLecturaRepository
    {
        Task<ResultadoTransaccionEntity<DocumentoLecturaEntity>> GetListPendienteByObjTypeAndCardCode(FilterRequestEntity value);
    }
}
