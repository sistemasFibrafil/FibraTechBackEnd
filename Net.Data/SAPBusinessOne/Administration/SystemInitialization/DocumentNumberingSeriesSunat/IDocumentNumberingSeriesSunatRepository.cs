using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDocumentNumberingSeriesSunatRepository
    {
        Task<ResultadoTransaccionResponse<DocumentNumberingSeriesSunatQueryEntity>> GetListSerieDocumento(DocumentNumberingSeriesSunatFindEntity value);
        Task<ResultadoTransaccionResponse<DocumentNumberingSeriesSunatQueryEntity>> GetNumeroDocumentoByTipoSerie(DocumentNumberingSeriesSunatEntity value);
    }
}
