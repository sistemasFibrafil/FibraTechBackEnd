using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDocumentTypeSunatRepository
    {
        Task<ResultadoTransaccionResponse<DocumentTypeSunatEntity>> GetListByType(DocumentTypeSunatEntity value);
    }
}
