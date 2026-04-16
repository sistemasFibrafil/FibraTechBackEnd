using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne.Administration
{
    public interface IDocumentSeriesConfigurationRepository
    {
        Task<ResultadoTransaccionEntity<DocumentSeriesConfigurationQueryEntity>> GetById(DocumentSeriesConfigurationFindEntity value);
        Task<ResultadoTransaccionEntity<DocumentSeriesConfigurationEntity>> SetCreate(DocumentSeriesConfigurationCreateEntity value);
    }
}
