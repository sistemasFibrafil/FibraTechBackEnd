using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDocumentNumberingSeriesRepository
    {
        Task<ResultadoTransaccionEntity<DocumentNumberingSeries1Entity>> GetNumero(DocumentNumberingSeriesFindEntity value);
    }
}
