using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDocumentNumberingSeriesRepository
    {
        Task<ResultadoTransaccionResponse<DocumentNumberingSeries1Entity>> GetNumero(DocumentNumberingSeriesFindEntity value);
    }
}
