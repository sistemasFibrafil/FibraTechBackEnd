using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDocumentNumberingSeriesSunatRepository
    {
        Task<ResultadoTransaccionEntity<DocumentNumberingSeriesSunatQueryEntity>> GetListSerieDocumento(DocumentNumberingSeriesSunatFindEntity value);
        Task<ResultadoTransaccionEntity<DocumentNumberingSeriesSunatQueryEntity>> GetNumeroDocumentoByTipoSerie(DocumentNumberingSeriesSunatEntity value);
    }
}
