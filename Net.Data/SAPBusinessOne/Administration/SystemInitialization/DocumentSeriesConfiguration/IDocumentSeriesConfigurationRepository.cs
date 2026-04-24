using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Find;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Query;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Entities;
namespace Net.Data.SAPBusinessOne.Administration
{
    public interface IDocumentSeriesConfigurationRepository
    {
        Task<ResultadoTransaccionResponse<DocumentSeriesConfigurationQueryEntity>> GetById(DocumentSeriesConfigurationFindEntity value);
        Task<ResultadoTransaccionResponse<DocumentSeriesConfigurationEntity>> SetCreate(DocumentSeriesConfigurationCreateEntity value);
    }
}
