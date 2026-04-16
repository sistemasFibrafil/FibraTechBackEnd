using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDocumentTypeSunatRepository
    {
        Task<ResultadoTransaccionEntity<DocumentTypeSunatEntity>> GetListByType(DocumentTypeSunatEntity value);
    }
}
